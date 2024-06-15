using SchedulerApi.DAL.Queries;
using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.Entities;

public class ShiftException : IMyQueryable
{
    private Desk _desk;

    public Desk Desk
    {
        get => _desk;
        set
        {
            _desk = value;
            DeskId = value.Id;
        }
    }
    public string DeskId { get; set; }
    
    public DateTime ShiftStartDateTime { get; set; }

    private Shift _shift;

    public Shift Shift
    {
        get => _shift;
        set
        {
            _shift = value;
            Desk = value.Desk;
        }
    }

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }

    public ExceptionType ExceptionType { get; set; }
    public string? Reason { get; set; }

    public DateTime ModificationDateTime { get; set; }
    public User ModificationUser { get; set; }
    
    public object Key => new { DeskId, ShiftStartDateTime, EmployeeId };

  
    public static IEnumerable<string> QueryPropertyNames { get; } =
        new[] { "ShiftExceptionExceptionType", "ShiftExceptionReason" };

    public Dictionary<string, object?> QueryProperties => new()
    {
        { "ShiftExceptionExceptionType", ExceptionType },
        { "ShiftExceptionReason", Reason }
    };

    public Dictionary<string, object?> NavigationPropertyKeys => new()
    {
        { "Shift", (DeskId, ShiftStartDateTime) },
        { "Employee", EmployeeId }
    };

    public static Dictionary<string, Type> NavigationPropertyTypes { get; } = new()
    {
        { "Shift", typeof(Shift) },
        { "Employee", typeof(Employee) },
    };
}