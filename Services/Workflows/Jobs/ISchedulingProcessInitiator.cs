namespace SchedulerApi.Services.Workflows.Jobs;

public interface ISchedulingProcessInitiator
{
    Task<bool> CheckAndInitiateProcessAsync(string deskId, string strategyName = "gpt");
}