using SchedulerApi.Services.Workflows.Processes.Interfaces;

namespace SchedulerApi.Services.Workflows.Scanners;

public interface IAutoScheduleScanner
{
    IAutoScheduleProcess? ProcessInProgress { get; }

    TimeSpan CycleDuration { get; }
    TimeSpan CatchRangeDuration { get; }
    bool ShouldRun { get; set; }
    Task Run();
    Task Wake();
    void Terminate();
}
