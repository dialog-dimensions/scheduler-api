namespace SchedulerApi.Services.Workflows.Processes.Interfaces;

public interface IAutoScheduleProcess : IProcess
{
    public DateTime ProcessStart { get; }
    public DateTime FileWindowEnd { get; }
    public DateTime PublishDateTime { get; }
    
    Task Initialize(DateTime startDateTime, DateTime endDateTime, int shiftDuration);
    Task Run(DateTime startDateTime, DateTime endDateTime, int shiftDuration);
}
