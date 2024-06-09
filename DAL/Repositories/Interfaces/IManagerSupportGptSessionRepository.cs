using SchedulerApi.Models.ChatGPT.Sessions;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IManagerSupportGptSessionRepository : IRepository<ManagerSupportGptSession>
{
    Task<ManagerSupportGptSession?> FindByManagerIdAsync(int managerId);
}