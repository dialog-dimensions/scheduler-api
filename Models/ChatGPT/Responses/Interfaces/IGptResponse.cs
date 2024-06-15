namespace SchedulerApi.Models.ChatGPT.Responses.Interfaces;

public interface IGptResponse
{
    string StatusCode { get; set; }
    object? Content { get; set; }
    
    public bool IsSuccessStatusCode => StatusCode[0] == '2';
}
