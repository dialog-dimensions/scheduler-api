using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ScheduleCommands;

public class ReadSchedulesCommand : ReadManyCommand<Schedule>
{
    public ReadSchedulesCommand(IQueryService queryService) : base(queryService)
    {
        
    }
}
