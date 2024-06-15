using SchedulerApi.Enums;

namespace SchedulerApi.Models.ChatGPT.Requests.Interfaces;

public interface IGptRequest
{
    GptRequestType GptRequestType { get; }
    Dictionary<string, object> Parameters { get; }
}
