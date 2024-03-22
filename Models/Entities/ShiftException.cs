using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.Entities;

public class ShiftException : IKeyProvider
{
    public DateTime ShiftKey { get; set; }
    public Shift Shift { get; set; }

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }

    public ExceptionType ExceptionType { get; set; }
    public string? Reason { get; set; }

    public DateTime ModificationDateTime { get; set; }
    public User ModificationUser { get; set; }
    
    public object Key => new { ShiftKey, EmployeeId };
}