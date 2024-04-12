using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IShiftRepository : IRepository<Shift>
{
    Task UpdateShiftEmployeeAsync(string deskId, DateTime shiftStart, Employee employee);
}