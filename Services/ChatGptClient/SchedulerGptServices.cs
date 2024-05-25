﻿using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic.CompilerServices;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT;
using SchedulerApi.Services.ChatGptClient.Interfaces;
using SchedulerApi.Services.WhatsAppClient.Twilio;

namespace SchedulerApi.Services.ChatGptClient;

public class SchedulerGptServices : ISchedulerGptServices
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ISchedulerGptSessionRepository _sessionRepository;
    private readonly IShiftExceptionRepository _exceptionRepository;
    private readonly IAssistantServices _assistantServices;
    private readonly ITwilioServices _twilioServices;

    private string AssistantId { get; set; }

    public SchedulerGptServices(
        UserManager<IdentityUser> userManager,
        IScheduleRepository scheduleRepository,
        IEmployeeRepository employeeRepository,
        ISchedulerGptSessionRepository sessionRepository,
        IShiftExceptionRepository exceptionRepository,
        IAssistantServices assistantServices,
        ITwilioServices twilioServices,
        IConfiguration configuration
        )
    {
        _userManager = userManager;
        _scheduleRepository = scheduleRepository;
        _employeeRepository = employeeRepository;
        _sessionRepository = sessionRepository;
        _exceptionRepository = exceptionRepository;
        _assistantServices = assistantServices;
        _twilioServices = twilioServices;
        AssistantId = configuration["ChatGPT:AssistantId"]!;
    }
    
    public async Task<string> CreateSession((string, DateTime) scheduleKey, int employeeId, Dictionary<string, string>? otherInstructions = null)
    {
        // Get Schedule and Employee Records
        var schedule = await _scheduleRepository.ReadAsync(scheduleKey);
        if (schedule is null)
        {
            throw new KeyNotFoundException("unable to retrieve schedule.");
        }

        var employee = await _employeeRepository.ReadAsync(employeeId);
        if (employee is null)
        {
            throw new KeyNotFoundException("unable to retrieve employee.");
        }

        var user = await _userManager.FindByIdAsync(employeeId.ToString());
        if (user is null)
        {
            throw new KeyNotFoundException("unable to retrieve user.");
        }
        
        // Create GPT Thread
        var threadId = await _assistantServices.CreateThreadAsync();
        
        // Save A New Session to the Database
        await _sessionRepository.CreateAsync(new SchedulerGptSession
        {
            ThreadId = threadId,
            Schedule = schedule,
            Employee = employee,
            ConversationState = ShabtzanGptConversationState.Created
        });
        
        // Generate the Initial Instructions Message and Process It Without Replying the User
        var initialInstructionsMessage = SchedulerGptUtils.InitialStringBuilder(schedule, employee, otherInstructions);
        await ProcessIncomingMessage(threadId, initialInstructionsMessage, true);
        
        // Return the New Thread ID
        return threadId;
    }

    public async Task ProcessIncomingMessage(string threadId, string incomingMessage, bool initialContact = false)
    {
        // Add Message to the GPT
        if (!await _assistantServices.AddMessageToThreadAsync(threadId, incomingMessage))
        {
            throw new ApplicationException("error adding message to the thread");
        }
        
        // Run the GPT
        if (!await _assistantServices.RunThreadAsync(threadId, AssistantId))
        {
            throw new ApplicationException("error running the thread in the assistant");
        }

        await Task.Delay(30000);

        // Load Session
        var session = await _sessionRepository.ReadAsync(threadId);
        if (session is null)
        {
            return;
        }

        // Analyze New Conversation State According to Outgoing Message
        var newConversationState = initialContact ? 
            ShabtzanGptConversationState.Gathering : 
            SchedulerGptUtils.AnalyzeConversationState(session.LatestMessage.Content);

        // If Applicable, Process New State
        if (newConversationState is not null)
        {
            session.ConversationState = newConversationState.Value;
            await _sessionRepository.UpdateAsync(session);
            
            // If Applicable, Process JSON to Extract ShiftException instances and Update DB.
            if (session.ConversationState == ShabtzanGptConversationState.JsonDetected)
            {
                var exceptions = SchedulerGptUtils.GetShiftExceptions(session.LatestMessage.Content);
                await _exceptionRepository.CreateRangeAsync(exceptions);
            }
        }
        
        // Get User Phone Number from User Record
        var user = await _userManager.FindByIdAsync(session.EmployeeId.ToString());
        if (user is null)
        {
            return;
        }
        
        var phoneNumber = user.PhoneNumber!;

        if (initialContact)
        {
            await _twilioServices.TriggerCallToFileGptFlow(
                phoneNumber, 
                session.Employee!.Name, 
                session.Employee!.Gender, 
                session.Schedule!,
                DateTime.MaxValue
                );
        }
        else
        {
            // Determine Reply for User
            var reply = DetermineReply(session);
        
            // Send Reply to User
            await _twilioServices.SendFreeFormMessage(reply, phoneNumber);
            
            // Finally, Update Conversation State for Signoff
            if (session.ConversationState == ShabtzanGptConversationState.JsonDetected)
            {
                session.ConversationState = ShabtzanGptConversationState.Ended;
                await _sessionRepository.UpdateAsync(session);
            }
        }
    }

    private string DetermineReply(SchedulerGptSession session) =>
        (session.ConversationState >= ShabtzanGptConversationState.JsonDetected) switch
        {
            true => OutgoingUserMessageWhenJsonDetected(session.LatestMessage.Content),
            false => session.LatestMessage.Content
        };

    private string OutgoingUserMessageWhenJsonDetected(string rawMessage)
    {
        return SchedulerGptUtils.GetSubstringPriorToFlag(
            rawMessage, 
            rawMessage.Contains(SchedulerGptUtils.EndGatherFlag) ? 
                SchedulerGptUtils.EndGatherFlag : 
                SchedulerGptUtils.StartJsonFlag);
    }
}
