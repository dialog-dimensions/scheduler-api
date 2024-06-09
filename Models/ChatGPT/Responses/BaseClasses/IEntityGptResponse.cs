using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.ChatGPT.Responses.BaseClasses;

public interface IEntityGptResponse<T, TDto> : IGptResponse where T : IKeyProvider where TDto : IDto<T, TDto>
{
    TDto Entity { get; set; }
}
