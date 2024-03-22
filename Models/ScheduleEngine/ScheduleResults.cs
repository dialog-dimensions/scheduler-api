using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.ScheduleEngine;

public class ScheduleResults
{
    public Schedule CompleteSchedule { get; set; }
    public ScheduleReport Report { get; set; }
}
