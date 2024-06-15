namespace SchedulerApi.Models.ChatGPT.Responses.Interfaces;

public interface IEntityGptResponse : IGptResponse
{
    object? Entity { set; }
}
