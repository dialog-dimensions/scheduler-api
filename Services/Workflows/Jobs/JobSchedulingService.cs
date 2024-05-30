using Hangfire;
using Hangfire.Common;

namespace SchedulerApi.Services.Workflows.Jobs;

public class JobSchedulingService : IJobSchedulingService
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public JobSchedulingService(IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient)
    {
        _recurringJobManager = recurringJobManager;
        _backgroundJobClient = backgroundJobClient;
    }

    public void InitializeRecurringJob(string jobId, string cronExpression)
    {
        _recurringJobManager.AddOrUpdate(
            jobId,
            () => Console.WriteLine("hello world job"),
            cronExpression);
    }

    public void RemoveRecurringJob(string jobId)
    {
        _recurringJobManager.RemoveIfExists(jobId);
    }

    public void ScheduleDelayedJob(TimeSpan timeSpan, Action action)
    {
        var jobId = _backgroundJobClient.Schedule(
            () => action.Invoke(), 
            timeSpan);
    }
}
