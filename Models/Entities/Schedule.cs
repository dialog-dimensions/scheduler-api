using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.Entities;

public class Schedule : List<Shift>, IKeyProvider
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int ShiftDuration { get; set; }
    public object Key => StartDateTime;

    public bool IsFullyScheduled => this.All(shift => shift.EmployeeId is > 0);
}