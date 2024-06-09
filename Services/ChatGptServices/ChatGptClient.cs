using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using SchedulerApi.Models.ChatGPT;

namespace SchedulerApi.Services.ChatGptServices;

public class ChatGptClient(IOpenAIService openAiService) : IChatGptClient
{
    public async Task<string> CreateThreadAsync()
    {
        var response = await openAiService.Beta.Threads.ThreadCreate();
        return response.Id;
    }

    public async Task<bool> AddMessageToThreadAsync(string threadId, string message)
    {
        var request = new MessageCreateRequest
        {
            Role = "user", 
            Content = new MessageContentOneOfType
            {
                AsString = message
            }
        };

        var response = await openAiService.Beta.Messages.CreateMessage(threadId, request);
        return response.Successful;
    }

    public async Task<bool> RunThreadAsync(string threadId, string assistantId)
    {
        var request = new RunCreateRequest
        {
            AssistantId = assistantId
        };
        
        var response = await openAiService.Beta.Runs.RunCreate(threadId, request);
        return response.Successful;
    }

    public async Task<IEnumerable<Message>> ThreadListMessagesAsync(string threadId)
    {
        var response = await openAiService.Beta.Messages.ListMessages(threadId);
        return response.Data!.Select(msgRes => new Message
        {
            Role = msgRes.Role, 
            Content = msgRes.Content![0].Text!.Value,
            TimeStamp = msgRes.CreatedAt
        });
    }

    public async Task<Message> ReadLatestMessageAsync(string threadId)
    {
        var messages = await ThreadListMessagesAsync(threadId);
        return messages.MaxBy(msg => msg.TimeStamp)!;
    }
}