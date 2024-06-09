using Newtonsoft.Json;
using SchedulerApi.Models.ChatGPT.Responses.BaseClasses;

namespace SchedulerApi.Models.ChatGPT.Responses;

public class MessageGptResponse : GptResponse, IMessageGptResponse
{
    [JsonIgnore]
    public string ResponseMessage
    {
        get => Content.ToString()!;
        set => Content = new { ResponseMessage = value };
    }
}
