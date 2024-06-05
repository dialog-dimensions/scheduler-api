using SchedulerApi.Models.Interfaces;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.Entities;

public class Schedule : List<Shift>, IKeyProvider
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
}
