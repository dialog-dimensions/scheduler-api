using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Services.ChatGptClient.Interfaces;

public interface ISchedulerGptServices
{
    Task<string> CreateSession((string, DateTime) scheduleKey, int employeeId, Dictionary<string, string>? otherInstructions = null);
    Task ProcessIncomingMessage(string threadId, string incomingMessage, bool initialMessage = false);
}
