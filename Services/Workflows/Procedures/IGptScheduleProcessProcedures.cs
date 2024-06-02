namespace SchedulerApi.Services.Workflows.Procedures;

public interface IGptScheduleProcessProcedures
{
    Task CheckAndInitiateProcessAsync(string deskId);
    Task AfterGatheringAsync(int processId);
    Task AfterApprovalAsync(int processId);
}