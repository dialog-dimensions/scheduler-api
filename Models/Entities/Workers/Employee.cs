using SchedulerApi.Models.Entities.Workers.BaseClasses;

namespace SchedulerApi.Models.Entities.Workers;

public class Employee : Worker
{
    public Employee()
    {
        Role = "Employee";
    }
    
    public double Balance { get; set; }
    public double DifficultBalance { get; set; }
    public bool Active { get; set; } = true;
}
