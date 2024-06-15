using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ShiftExceptionCommands;

public class ReadShiftExceptionCommand : ReadCommand<ShiftException>
{
    public ReadShiftExceptionCommand(IQueryService queryService) : base(queryService)
    {
    }
}
