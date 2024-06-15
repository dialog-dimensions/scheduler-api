using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.DeskCommands;

public class ReadDesksCommand : ReadManyCommand<Desk>
{
    public ReadDesksCommand(IQueryService queryService) : base(queryService)
    {
    }
}