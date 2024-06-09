using SchedulerApi.Enums;

namespace SchedulerApi.Models.ChatGPT.Requests.BaseClasses;

public interface IGptRequest
{
    GptRequestType GptRequestType { get; }
    Dictionary<string, object> Parameters { get; }
}
