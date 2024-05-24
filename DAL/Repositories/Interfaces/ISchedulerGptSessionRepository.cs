using SchedulerApi.Models.ChatGPT;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface ISchedulerGptSessionRepository : IRepository<SchedulerGptSession>
{
    Task<SchedulerGptSession?> FindActiveByEmployeeIdAsync(int employeeId);
}