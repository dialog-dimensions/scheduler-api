using SchedulerApi.Models.Entities;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IShiftExceptionRepository : IRepository<ShiftException>
{
    Task<IEnumerable<ShiftException>> WhereAsync(string deskId, DateTime shiftStartDateTime);
    Task<IEnumerable<ShiftException>> WhereAsync(int employeeId);

    Task<IEnumerable<ShiftException>> GetScheduleExceptions(string deskId, DateTime scheduleStartDateTime);
    Task CreateRangeAsync(IEnumerable<ShiftException> exceptions);
}