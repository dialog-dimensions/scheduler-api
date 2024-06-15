using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ShiftCommands;

public class ReadShiftCommand : ReadCommand<Shift>
{
    public ReadShiftCommand(IQueryService queryService) : base(queryService)
    {
    }
}
