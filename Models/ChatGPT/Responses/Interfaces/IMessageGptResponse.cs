namespace SchedulerApi.Models.ChatGPT.Responses.Interfaces;

public interface IMessageGptResponse : IGptResponse
{
    string ResponseMessage { get; set; }
}