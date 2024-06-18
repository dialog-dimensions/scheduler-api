using SchedulerApi.DAL.Queries;
using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.Entities;

public class Shift : IMyQueryable
{
    
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

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
    
    public string DeskId { get; private set; }
    
    public DateTime ScheduleStartDateTime { get; set; }
    
    private Employee? _employee;
    public Employee? Employee
    {
        get => _employee;
        set
        {
            if (value is null)
            {
                if (_employee is not null)
                {
                    _employee = null;
                    EmployeeId = 0;
                }

                return;
            }

            if (_employee?.Equals(value) ?? false)
            {
                return;
            }
            
            _employee = value;
            EmployeeId = value.Id;
        }
    }
    public int? EmployeeId { get; private set; }

    public object Key => new {DeskId, StartDateTime};

    public DateTime ModificationDateTime { get; set; }
    public User ModificationUser { get; set; }

    private bool IsWeekend => EndDateTime.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday;

    public bool IsDifficult => IsWeekend;

    public Shift Copy() => new()
    {
        StartDateTime = StartDateTime,
        EndDateTime = EndDateTime,
        Desk = Desk,
        ScheduleStartDateTime = ScheduleStartDateTime,
        Employee = Employee,
        EmployeeId = EmployeeId,
    };


    public static IEnumerable<string> QueryPropertyNames { get; } = new[]
        { "ShiftStartDateTime", "ShiftEndDateTime", "ShiftIsWeekend", "ShiftIsDifficult" };

    public Dictionary<string, object?> QueryProperties => new()
    {
        { "ShiftStartDateTime", StartDateTime },
        { "ShiftEndDateTime", EndDateTime },
        { "ShiftIsWeekend", IsWeekend },
        { "ShiftIsDifficult", IsDifficult }
    };

    public Dictionary<string, object?> NavigationPropertyKeys => new()
    {
        { "Desk", DeskId },
        { "Schedule", (DeskId, ScheduleStartDateTime) }
    };

    public static Dictionary<string, Type> NavigationPropertyTypes { get; } = new()
    {
        { "Desk", typeof(Desk) },
        { "Schedule", typeof(Schedule) },
        { "Employee", typeof(Employee) }
    };
    
    
}
