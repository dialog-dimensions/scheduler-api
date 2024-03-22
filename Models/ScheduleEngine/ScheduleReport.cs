using SchedulerApi.Models.DTOs;
using SchedulerApi.Models.DTOs.ScheduleEngineModels;

namespace SchedulerApi.Models.ScheduleEngine;

public class ScheduleReport
{
    public ScheduleQuotasDto Quotas { get; set; }
    public IEnumerable<ShiftExceptionDto> Violations { get; set; }
    public IEnumerable<EmployeeIncrements> Increments { get; set; }
    public double Score { get; set; }
}