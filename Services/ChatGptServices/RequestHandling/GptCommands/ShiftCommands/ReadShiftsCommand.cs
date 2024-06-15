using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ShiftCommands;

public class ReadShiftsCommand : ReadManyCommand<Shift>
{
    public ReadShiftsCommand(IQueryService queryService) : base(queryService)
    {
    }
}
