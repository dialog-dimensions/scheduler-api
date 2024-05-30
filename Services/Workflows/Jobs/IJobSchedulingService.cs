namespace SchedulerApi.Services.Workflows.Jobs;

public interface IJobSchedulingService
{
    void InitializeRecurringJob(string jobId, string cronExpression);
    void RemoveRecurringJob(string jobId);
    void ScheduleDelayedJob(TimeSpan timeSpan, Action action);

    public const string ScannerJobId = "scanner";
    public const string ScannerCron = "0 0 10 * * *"; // Run daily at 10am.
}
