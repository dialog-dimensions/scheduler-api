using SchedulerApi.Services.Workflows.Processes.Classes;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IAutoScheduleProcessRepository : IRepository<AutoScheduleProcess>
{
    Task<AutoScheduleProcess?> ReadRunningAsync(string deskId, DateTime scheduleStart);
}