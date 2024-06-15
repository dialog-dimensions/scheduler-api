using SchedulerApi.DAL.Queries;
using SchedulerApi.Models.Entities;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ShiftExceptionCommands;

public class ReadShiftExceptionsCommand : ReadManyCommand<ShiftException>
{
    public ReadShiftExceptionsCommand(IQueryService queryService) : base(queryService)
    {
    }
}
