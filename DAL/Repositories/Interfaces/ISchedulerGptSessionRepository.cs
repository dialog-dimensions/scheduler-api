using SchedulerApi.Models.ChatGPT;
using SchedulerApi.Models.ChatGPT.Sessions;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface ISchedulerGptSessionRepository : IRepository<GathererGptSession>
{
    Task<GathererGptSession?> FindActiveByEmployeeIdAsync(int employeeId);
}