using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IDeskRepository : IRepository<Desk>
{
    Task<IEnumerable<Desk>> ReadAllActiveUnit(Unit unit);
    Task<IEnumerable<Desk>> ReadAllUnit(Unit unit);
    Task<IEnumerable<Desk>> ReadAllUnit(string unitId);
    Task<IEnumerable<Employee>> GetDeskEmployees(Desk desk);
    Task<IEnumerable<Employee>> GetDeskEmployees(string deskId);
    Task<IEnumerable<Desk>> GetEmployeeDesks(Employee employee);
    Task<IEnumerable<Desk>> GetEmployeeDesks(int employeeId);

    Task AddDeskAssignment(Desk desk, Employee employee);
    Task RemoveDeskAssignment(Desk desk, Employee employee);

    Task RemoveDeskAssignments(Desk desk);
    Task RemoveEmployeeAssignments(Employee employee);
    Task UpdateProcessParametersAsync(string deskId, string catchRangeString, string fileWindowDurationString,
        string headsUpDurationString);
}
