using SchedulerApi.Services.Workflows.Processes;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IProcessRepository : IRepository<Process>
{
    Task<IEnumerable<Process>> ReadRunningAsync();
}