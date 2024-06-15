using SchedulerApi.DAL.Queries;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Models.Organization;

public class DeskAssignment : IMyQueryable
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
    public string DeskId { get; private set; }

    private Employee _employee;
    public Employee Employee
    {
        get => _employee;
        set
        {
            _employee = value;
            EmployeeId = value.Id;
        }
    }
    public int EmployeeId { get; private set; }

    public object Key => new { DeskId, EmployeeId };

    public static IEnumerable<string> QueryPropertyNames { get; } = new string[] { };
    public Dictionary<string, object?> QueryProperties => new();

    public Dictionary<string, object?> NavigationPropertyKeys => new()
    {
        { "Desk", DeskId },
        { "Employee", EmployeeId }
    };

    public static Dictionary<string, Type> NavigationPropertyTypes { get; } = new()
    {
        { "Desk", typeof(Desk) },
        { "Employee", typeof(Employee) }
    };
}
