using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ScheduleEngine.Interfaces;

public interface IScheduleScorer
{
    double Score(ScheduleData data);
}