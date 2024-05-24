using SchedulerApi.Models.ChatGPT;

namespace SchedulerApi.Services.ChatGptClient.Interfaces;

public interface IAssistantServices
{
    Task<string> CreateThreadAsync();
    Task<bool> AddMessageToThreadAsync(string threadId, string message);
    Task<bool> RunThreadAsync(string threadId, string assistantId);
    Task<IEnumerable<Message>> ThreadListMessagesAsync(string threadId);
    Task<Message> ReadLatestMessageAsync(string threadId);
}
