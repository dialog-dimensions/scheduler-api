using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.Entities;

public class Shift : IKeyProvider
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime ScheduleKey { get; set; }
    
    private Employee? _employee;
    public Employee? Employee
    {
        get => _employee;
        set
        {
            if (value is null && _employee is null) return;
            if (_employee?.Equals(value) ?? false) return;
            _employee = value;
            EmployeeId = _employee?.Id ?? 0;
        }
    }
    public int? EmployeeId { get; private set; }

    public object Key => StartDateTime;

    public DateTime ModificationDateTime { get; set; }
    public User ModificationUser { get; set; }

    private bool IsWeekend => EndDateTime.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday;

    public bool IsDifficult => IsWeekend;

    public Shift Copy() => new()
    {
        StartDateTime = StartDateTime,
        EndDateTime = EndDateTime,
        ScheduleKey = ScheduleKey,
        Employee = Employee,
        EmployeeId = EmployeeId,
    };
}
