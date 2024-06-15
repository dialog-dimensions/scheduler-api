using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ScheduleCommands;

public class ReadScheduleCommand : ReadCommand<Schedule>
{
    public ReadScheduleCommand(IQueryService queryService) : base(queryService)
    {
    }
}