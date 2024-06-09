using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Responses;
using SchedulerApi.Models.ChatGPT.Responses.BaseClasses;
using SchedulerApi.Models.ChatGPT.Sessions;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ChatGptServices.Assistants.Interfaces;
using SchedulerApi.Services.ChatGptServices.RequestHandlers;
using SchedulerApi.Services.ChatGptServices.RequestParser;
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
    private readonly IGptRequestHandler _requestHandler;
    private readonly IConfigurationSection _managerSupportAssistants;
    private readonly ITwilioServices _twilioServices;
    private readonly UserManager<IdentityUser> _userManager;

    public ManagerSupportServices(IManagerSupportGptSessionRepository sessionRepository, IGptRequestParser requestParser, IGptRequestHandler requestHandler, IChatGptClient chatGptClient, IConfiguration configuration, ITwilioServices twilioServices, UserManager<IdentityUser> userManager)
    {
        _sessionRepository = sessionRepository;
        _requestParser = requestParser;
        _requestHandler = requestHandler;
        _chatGptClient = chatGptClient;
        _twilioServices = twilioServices;
        _userManager = userManager;
        _managerSupportAssistants = configuration.GetSection("ChatGPT:Assistants:ManagerSupport"); // TODO: check if this is not fucked up in prod.
    }

    public async Task ProcessIncomingMessage(Employee manager, string incomingMessage)
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

        // Add message to the thread
        await _chatGptClient.AddMessageToThreadAsync(session.ThreadId, incomingMessage);

        // Run the thread
        await _chatGptClient.RunThreadAsync(session.ThreadId, session.CurrentAssistantId);
        await Task.Delay(10000);

        // Get GPT response
        var latestMessage = await _chatGptClient.ReadLatestMessageAsync(session.ThreadId);

        // Identify request triggers
        while (latestMessage.Content.Contains(RequestStartFlag))
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
    
            // Send response back to the GPT
            await _chatGptClient.AddMessageToThreadAsync(session.ThreadId, responseSerialization);

            // Run the GPT again
            await _chatGptClient.RunThreadAsync(session.ThreadId, session.CurrentAssistantId);
            await Task.Delay(10000);

            // Refresh latest message
            latestMessage = await _chatGptClient.ReadLatestMessageAsync(session.ThreadId);
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
            await _chatGptClient.RunThreadAsync(session.ThreadId, session.CurrentAssistantId);
            await Task.Delay(10000);
            
            // Refresh latest message
            latestMessage = await _chatGptClient.ReadLatestMessageAsync(session.ThreadId);
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
            return await _requestHandler.HandleRequest(request);
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