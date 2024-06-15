using SchedulerApi.DAL.Queries;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.Entities;

public class Schedule : List<Shift>, IMyQueryable
{
    public Desk Desk => SomeShift.Desk;
    public string DeskId => Desk.Id;
    public DateTime StartDateTime => FirstShift.StartDateTime;
    public DateTime EndDateTime => LastShift.EndDateTime;
    public int ShiftDuration => (int)(Duration.TotalHours / Count);
    public object Key => new {DeskId, StartDateTime};
    public bool IsFullyScheduled => this.All(shift => shift.EmployeeId is > 0);
    
    public Shift FirstShift => this.MinBy(s => s.StartDateTime)!;
    public Shift LastShift => this.MaxBy(s => s.StartDateTime)!;
    private Shift SomeShift => this[0];
    public TimeSpan Duration => EndDateTime.Subtract(StartDateTime);
    
    public static IEnumerable<string> QueryPropertyNames { get; } = new[]
        { "ScheduleStartDateTime", "ScheduleEndDateTime", "ScheduleShiftDuration", "ScheduleIsFullyScheduled" };

    public Dictionary<string, object?> QueryProperties => new()
    {
        { "ScheduleStartDateTime", StartDateTime },
        { "ScheduleEndDateTime", EndDateTime },
        { "ScheduleShiftDuration", ShiftDuration },
        { "ScheduleIsFullyScheduled", IsFullyScheduled }
    };

    public Dictionary<string, object?> NavigationPropertyKeys => new()
    {
        { "Desk", DeskId }
    };

    public static Dictionary<string, Type> NavigationPropertyTypes { get; } = new() { { "Desk", typeof(Desk) } };
}
