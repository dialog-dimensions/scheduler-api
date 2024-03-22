using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ScheduleEngine.Interfaces;

public interface IScheduleReportBuilder
{
    ScheduleReport BuildReport(ScheduleData data);
}
