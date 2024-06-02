namespace SchedulerApi.Services.Workflows.Jobs.Interfaces;

public interface IGptProcessAfterGatheringJob
{
    Task Execute(int processId);
}
