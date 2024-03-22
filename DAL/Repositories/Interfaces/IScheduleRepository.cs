using SchedulerApi.Models.Entities;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IScheduleRepository : IRepository<Schedule>
{
    Task<Schedule?> ReadLatestAsync();
    Task<Schedule?> ReadCurrentAsync();
    Task<Schedule?> ReadNextAsync();
    Task<ScheduleData> GetScheduleData(DateTime key);
    Task<ScheduleReport> GetScheduleReport(Schedule schedule);
    Task AssignEmployees(DateTime key, Schedule schedule);
    Task<Schedule?> ReadNearestIncomplete();

    // Task<IEnumerable<DateTime>> UpdateAsync(Schedule schedule);
}