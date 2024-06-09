using SchedulerApi.Models.ChatGPT.Responses.BaseClasses;
using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.ChatGPT.Responses;

public class EntityGptResponse<T, TDto> : GptResponse, IEntityGptResponse<T, TDto> where T : IKeyProvider where TDto : IDto<T, TDto>
{
    private TDto _entity;
    public TDto Entity
    {
        get => _entity;
        set
        {
            _entity = value;
            Content = value;
        }
    }
}
