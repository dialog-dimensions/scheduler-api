using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.ScheduleEngine;

public class EmployeeQuotas : IKeyProvider
{
    public Employee Employee { get; set; }
    public Schedule Schedule { get; set; }
    public double RegularQuota { get; set; }
    public double DifficultQuota { get; set; }

    public object Key => new { Schedule, Employee };
    public object Value => new { RegularQuota, DifficultQuota };
}