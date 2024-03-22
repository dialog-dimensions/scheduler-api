using SchedulerApi.Models.Entities;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IShiftExceptionRepository : IRepository<ShiftException>
{
    Task<IEnumerable<ShiftException>> WhereAsync(DateTime shiftKey);
    Task<IEnumerable<ShiftException>> WhereAsync(int employeeId);

    Task<IEnumerable<ShiftException>> GetScheduleExceptions(DateTime scheduleKey);
}