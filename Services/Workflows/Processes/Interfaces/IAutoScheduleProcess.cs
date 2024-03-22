namespace SchedulerApi.Services.Workflows.Processes.Interfaces;

public interface IAutoScheduleProcess : IProcess
{
    Task Initialize(DateTime startDateTime, DateTime endDateTime, int shiftDuration);
    Task Run(DateTime startDateTime, DateTime endDateTime, int shiftDuration);
}