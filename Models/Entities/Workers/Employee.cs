using SchedulerApi.Models.Entities.Workers.BaseClasses;

namespace SchedulerApi.Models.Entities.Workers;

public class Employee : Worker
{
    public Employee()
    {
        Role = "Employee";
    }

    public new IEnumerable<string> QueryPropertyNames { get; } =
        Worker.QueryPropertyNames.Concat(new[] { "EmployeeBalance", "EmployeeDifficultBalance", "EmployeeActive" });

    public new Dictionary<string, object?> QueryProperties =>
        base.QueryProperties.Concat(new Dictionary<string, object?>()).ToDictionary();

    public double Balance { get; set; }
    public double DifficultBalance { get; set; }
    public bool Active { get; set; } = true;
}
