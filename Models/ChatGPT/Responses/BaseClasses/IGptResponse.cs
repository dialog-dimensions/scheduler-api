namespace SchedulerApi.Models.ChatGPT.Responses.BaseClasses;

public interface IGptResponse
{
    string StatusCode { get; set; }
    object Content { get; set; }
}