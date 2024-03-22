using SchedulerApi.Models.Entities;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ScheduleEngine.Interfaces;

public interface IScheduler
{
    ScheduleData? Data { get; set; }
    Task<ScheduleResults> RunAsync(Schedule schedule);
}