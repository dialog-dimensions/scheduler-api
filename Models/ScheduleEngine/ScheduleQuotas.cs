using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.ScheduleEngine;

public class ScheduleQuotas : List<EmployeeQuotas>, IKeyProvider
{
    public Schedule Schedule { get; set; }
    public double ProjectedRegularQuota { get; set; }
    public double ProjectedDifficultQuota { get; set; }
    public object Key => Schedule;

    public Dictionary<Employee, double> GetRegularQuotas =>
        this.ToDictionary(eq => eq.Employee, eq => eq.RegularQuota);

    public Dictionary<Employee, double> GetDifficultQuotas =>
        this.ToDictionary(eq => eq.Employee, eq => eq.DifficultQuota);

    public double GetQuota(Employee employee)
    {
        return this.FirstOrDefault(dto => dto.Employee.Id.Equals(employee.Id))?.RegularQuota ?? 0.0;
    }

    public double GetDifficultQuota(Employee employee)
    {
        return this.FirstOrDefault(dto => dto.Employee.Id.Equals(employee.Id))?.DifficultQuota ?? 0.0;
    }
}