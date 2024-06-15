using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT;
using SchedulerApi.Models.ChatGPT.Responses;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Models.ChatGPT.Sessions;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ChatGptServices.Assistants.Interfaces;
using SchedulerApi.Services.ChatGptServices.RequestHandling.RequestDispatching;
using SchedulerApi.Services.ChatGptServices.RequestParsing;
using SchedulerApi.Services.ChatGptServices.Utils;
using SchedulerApi.Services.WhatsAppClient.Twilio;

namespace SchedulerApi.Services.ChatGptServices.Assistants;

public class ManagerSupportServices : IManagerSupportServices
{
    private const string RequestStartFlag = "//REQUEST START//";
    private const string RequestEndFlag = "//REQUEST END//";
    private const string RouteToOperatorFlag = "//OPERATOR//";
    private const string RouteToEmployeesFlag = "//EMPLOYEES//";
    private const string RouteToSchedulesFlag = "//SCHEDULES//";
    
    private readonly IChatGptClient _chatGptClient;
    private readonly IManagerSupportGptSessionRepository _sessionRepository;
    private readonly IGptRequestParser _requestParser;
    private readonly IRequestDispatcher _requestDispatcher;
    private readonly IConfigurationSection _managerSupportAssistants;
    private readonly ITwilioServices _twilioServices;
    private readonly UserManager<IdentityUser> _userManager;

    public ManagerSupportServices(IManagerSupportGptSessionRepository sessionRepository, IGptRequestParser requestParser, IGptRequestHandler requestHandler, IChatGptClient chatGptClient, IConfiguration configuration, ITwilioServices twilioServices, UserManager<IdentityUser> userManager)
    {
        _sessionRepository = sessionRepository;
        _requestParser = requestParser;
        _chatGptClient = chatGptClient;
        _twilioServices = twilioServices;
        _userManager = userManager;
        _requestDispatcher = requestDispatcher;
        _managerSupportAssistants = configuration.GetSection("ChatGPT:Assistants:ManagerSupport"); // TODO: check if this is not fucked up in prod.
    }

    public async Task ProcessIncomingMessage(Employee manager, string incomingMessage)
    {
        var session = await ReadOrCreateSession(manager);
        await SendRunProcess(session, incomingMessage, manager);
    }

    private async Task<ManagerSupportGptSession> ReadOrCreateSession(Employee manager)
    {
        // Check for existing open session with the manager
        var session = await _sessionRepository.FindByManagerIdAsync(manager.Id);

        // If not exists, create a new session
        if (session is null)
        {
            // Create a ChatGPT thread
            var threadId = await _chatGptClient.CreateThreadAsync();
            
            // Create a session thread
            session = new ManagerSupportGptSession
            {
                CurrentAssistantId = _managerSupportAssistants["SupportOperator"]!,
                State = GptSessionState.Created,
                ThreadId = threadId,
                Employee = manager
            };
            
            // Save new session in database
            await _sessionRepository.CreateAsync(session);
        }

        // Return the session
        return session;
    }

    private async Task SendRunProcess(ManagerSupportGptSession session, string message, Employee manager)
    {
        // Add message to the thread
        await _chatGptClient.AddMessageToThreadAsync(session.ThreadId, message);

        // Run and process
        await RunAndProcess(session, manager);
    }

    private async Task RunAndProcess(ManagerSupportGptSession session, Employee manager)
    {
        // Run the thread
        await _chatGptClient.RunThreadAsync(session.ThreadId, session.CurrentAssistantId);

        // Sample GPT response every 5 seconds for 10 tries
        Message? latestMessage = null;
        for (var t = 0; t < 10; t++)
        {
            await Task.Delay(5000);
            try
            {
                latestMessage = await _chatGptClient.ReadLatestMessageAsync(session.ThreadId);
                break;
            }
            catch
            {
                continue;
            }
        }

        // Unable to get response from GPT within a minute
        if (latestMessage is null)
        {
            await SendManagerAMessage(manager,
                "Couldn't get a response from the assistant. Please try again later or contact the system administrator.");
            return;
        }
        
        // Identify request triggers
        if (latestMessage.Content.Contains(RequestStartFlag))
        {
            // Send user any text generated prior to the request trigger
            var priorMessage = FuncTools.GetSubstringPriorToFlag(latestMessage.Content, RequestStartFlag);
            if (!string.IsNullOrEmpty(priorMessage))
            {
                await SendManagerAMessage(manager, priorMessage);
            }
            
            // Handle request and receive response
            var response = await HandleRequestTriggered(latestMessage.Content);
            
            // Serialize response
            var responseSerialization = JsonConvert.SerializeObject(response);

            // Send the message, run the thread and process the outcome.
            await SendRunProcess(session, responseSerialization, manager);

            return;
        }

        // Identify route triggers
        var routeTriggered = false;
        var newAssistantParamName = "";
        var routeTriggerPriorMessage = "";
        
        if (latestMessage.Content.Contains(RouteToOperatorFlag))
        {
            routeTriggered = true;
            newAssistantParamName = "SupportOperator";
            routeTriggerPriorMessage = FuncTools.GetSubstringPriorToFlag(latestMessage.Content, RouteToOperatorFlag);
        }
        
        else if (latestMessage.Content.Contains(RouteToEmployeesFlag))
        {
            routeTriggered = true;
            newAssistantParamName = "EmployeeManagement";
            routeTriggerPriorMessage = FuncTools.GetSubstringPriorToFlag(latestMessage.Content, RouteToEmployeesFlag);
        }
        
        else if (latestMessage.Content.Contains(RouteToSchedulesFlag))
        {
            routeTriggered = true;
            newAssistantParamName = "ScheduleManagement";
            routeTriggerPriorMessage = FuncTools.GetSubstringPriorToFlag(latestMessage.Content, RouteToSchedulesFlag);
        }

        if (routeTriggered)
        {
            // Send Prior Message to Manager
            await SendManagerAMessage(manager, routeTriggerPriorMessage);
            
            // Update Assistant in Session
            session.CurrentAssistantId = _managerSupportAssistants[newAssistantParamName]!;
            await _sessionRepository.UpdateAsync(session);
            
            // Run GPT again
            await RunAndProcess(session, manager);

            return;
        }
        
        // Send latest message back to the manager
        await SendManagerAMessage(manager, latestMessage.Content);
    }

    private async Task SendManagerAMessage(Employee manager, string message)
    {
        // Get User Record and Phone Number
        var user = await _userManager.FindByIdAsync(manager.Id.ToString());
        if (user is null)
        {
            throw new KeyNotFoundException("unable to find manager user record in database.");
        }

        var phoneNumber = user.PhoneNumber!;

        // Send Message to Manager
        await _twilioServices.SendFreeFormMessage(message, phoneNumber);
    }
    
    private async Task<IGptResponse> HandleRequestTriggered(string message)
    {
        try
        {
            // Trim request string
            var requestString = TrimRequestString(message);
            
            if (string.IsNullOrEmpty(requestString))
            {
                return new MessageGptResponse
                {
                    StatusCode = "400",
                    ResponseMessage =
                        "Problem with request flags. Both start flag and end flag must be present, with the request JSON schema between them."
                };
            }
            
            // Parse request
            var request = _requestParser.ParseRequest(requestString);

            // Execute request and receive response
            return await _requestDispatcher.HandleRequest(request);
        }
        catch (Exception ex)
        {
            return new MessageGptResponse
            {
                StatusCode = "400",
                ResponseMessage = ex.Message
            };
        }
    }

    private string TrimRequestString(string message)
    {
        var betweenFlags = FuncTools.GetSubstringBetweenEndpoints(message, RequestStartFlag, RequestEndFlag, false);
        if (betweenFlags.Length == 0)
        {
            return "";
        }
        
        var firstOpen = betweenFlags.IndexOf("{", StringComparison.CurrentCulture);
        var lastClose = betweenFlags.LastIndexOf("}", StringComparison.CurrentCulture);

        return betweenFlags.Substring(firstOpen, lastClose - firstOpen + 1);
    }
}