using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.UnitCommands;

public class ReadUnitsCommand : ReadManyCommand<Unit>
{
    public ReadUnitsCommand(IQueryService queryService) : base(queryService)
    {
    }
}
