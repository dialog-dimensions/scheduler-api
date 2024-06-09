namespace SchedulerApi.Models.ChatGPT.Responses.BaseClasses;

public interface IMessageGptResponse : IGptResponse
{
    string ResponseMessage { get; set; }
}