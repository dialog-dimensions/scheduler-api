using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ScheduleEngine.Interfaces;

public interface IDataGatherer
{
    Task<ScheduleData> GatherDataAsync(DateTime scheduleKey);
}