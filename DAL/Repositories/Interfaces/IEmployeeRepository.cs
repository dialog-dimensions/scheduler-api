using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> ReadAllActiveAsync();
    Task<IEnumerable<Employee>> ReadAllAssignedAsync();

    Task IncrementRegularBalance(int employeeId, double increment);
    Task IncrementDifficultBalance(int employeeId, double increment);
}