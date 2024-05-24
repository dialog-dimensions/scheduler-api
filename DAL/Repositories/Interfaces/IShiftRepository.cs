using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IShiftRepository : IRepository<Shift>
{
    Task UpdateShiftEmployeeAsync(string deskId, DateTime shiftStart, Employee employee);
    Task<IEnumerable<Shift>> GetEmployeeShiftsByRange(int employeeId, DateTime from, DateTime to);
    Task<IEnumerable<Shift>> GetDeskShiftsByRange(string deskId, DateTime from, DateTime to);

    Task<IEnumerable<Shift>> GetEmployeeShifts(int employeeId);
    Task<IEnumerable<Shift>> GetDeskShifts(string deskId);
}