using SchedulerApi.Models.Organization;

namespace SchedulerApi.Services.Workflows.Processes.Interfaces;

public interface IAutoScheduleProcess : IProcess
{
    public DateTime ProcessStart { get; }
    public DateTime FileWindowEnd { get; }
    public DateTime PublishDateTime { get; }

    public DateTime ScheduleStart { get; }
    public DateTime ScheduleEnd { get; }
    public int ScheduleShiftDuration { get; }

    Task Run(string deskId, DateTime startDateTime, DateTime endDateTime, int shiftDuration);
}
