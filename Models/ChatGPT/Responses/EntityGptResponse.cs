using SchedulerApi.Models.ChatGPT.Responses.BaseClasses;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;

namespace SchedulerApi.Models.ChatGPT.Responses;

public class EntityGptResponse : GptResponse, IEntityGptResponse
{
    public object? Entity
    {
        set => Content = value;
    }

    public static IEntityGptResponse Ok(object? entity) => new EntityGptResponse
    {
        StatusCode = "200",
        Entity = entity
    };

    public static IEntityGptResponse NoContent() => new EntityGptResponse
    {
        StatusCode = "204"
    };
}
