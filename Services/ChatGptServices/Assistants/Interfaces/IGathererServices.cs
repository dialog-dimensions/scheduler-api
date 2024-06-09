namespace SchedulerApi.Services.ChatGptServices.Assistants.Interfaces;

public interface IGathererServices
{
    Task<string> CreateSession((string, DateTime) scheduleKey, int employeeId, Dictionary<string, string>? otherInstructions = null);
    Task ProcessIncomingMessage(string threadId, string incomingMessage, bool initialMessage = false);
}
