namespace SchedulerApi.Services.Workflows.Jobs.Interfaces;

public interface IGptProcessAfterApprovalJob
{
    Task Execute(int processId);
}