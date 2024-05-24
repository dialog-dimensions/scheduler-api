using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.Entities;

public class ShiftException : IKeyProvider
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
    public Shift Shift { get; set; }

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }

    public ExceptionType ExceptionType { get; set; }
    public string? Reason { get; set; }

    public DateTime ModificationDateTime { get; set; }
    public User ModificationUser { get; set; }
    
    public object Key => new { DeskId, ShiftStartDateTime, EmployeeId };
}