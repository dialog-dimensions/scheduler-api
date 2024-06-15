using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.EmployeeCommands;

public class ReadEmployeeCommand : ReadCommand<Employee>
{
    public ReadEmployeeCommand(IQueryService queryService) : base(queryService)
    { }
}
