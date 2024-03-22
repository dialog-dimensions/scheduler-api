namespace SchedulerApi.Services.Workflows.Scanners;

public interface IAutoScheduleScanner
{
    TimeSpan CycleDuration { get; }
    TimeSpan CatchRangeDuration { get; }
    bool ShouldRun { get; set; }
    Task Run();
    Task Wake();
    void Terminate();
}
