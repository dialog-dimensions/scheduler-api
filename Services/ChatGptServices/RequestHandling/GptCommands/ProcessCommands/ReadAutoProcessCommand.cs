using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;
using SchedulerApi.Services.Workflows.Processes.Classes;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ProcessCommands;

public class ReadAutoProcessCommand : ReadCommand<AutoScheduleProcess>
{
    public ReadAutoProcessCommand(IQueryService queryService) : base(queryService)
    { }
}
