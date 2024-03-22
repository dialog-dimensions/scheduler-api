using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine;

public class ScheduleScorer : IScheduleScorer
{
    public double Score(ScheduleData data)
    {
        return 0.0;
    }
}
