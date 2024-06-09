namespace SchedulerApi.Models.ChatGPT.Responses.BaseClasses;

public class GptResponse : IGptResponse
{
    public string StatusCode { get; set; }
    public object Content { get; set; }
}