using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.DeskCommands;

public class ReadDeskCommand : ReadCommand<Desk>
{
    public ReadDeskCommand(IQueryService queryService) : base(queryService)
    { }
}
