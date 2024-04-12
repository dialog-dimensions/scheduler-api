using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Organization;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IScheduleRepository : IRepository<Schedule>
{
    Task<Schedule?> ReadLatestAsync(string deskId);
    Task<Dictionary<Desk, Schedule?>> ReadAllActiveLatest();
    Task<Schedule?> ReadCurrentAsync(string deskId);
    Task<Schedule?> ReadNextAsync(string deskId);
    Task<ScheduleData> GetScheduleData(string deskId, DateTime startDateTime);
    Task<ScheduleReport> GetScheduleReport(Schedule schedule);
    Task AssignEmployees(string deskId, DateTime startDateTime, Schedule schedule);
    Task<Schedule?> ReadNearestIncomplete(string deskId);

    // Task<IEnumerable<DateTime>> UpdateAsync(Schedule schedule);
}