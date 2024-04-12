using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IUnitRepository : IRepository<Unit>
{
    Task<Unit?> ReadByNameAsync(string name);
    Task<Organization?> GetUnderlyingOrganizationAsync(string id);
    Task<IEnumerable<Employee>> GetUnitEmployees(string id);
}
