using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Models.DTOs.ScheduleEngineModels;

public class EmployeeQuotasDto : IDto<EmployeeQuotas, EmployeeQuotasDto>
{
    public int EmployeeId { get; set; }
    public DateTime ScheduleKey { get; set; }
    public double RegularQuota { get; set; }
    public double DifficultQuota { get; set; }

    public static EmployeeQuotasDto FromEntity(EmployeeQuotas entity) => new()
    {
        EmployeeId = entity.Employee.Id,
        ScheduleKey = entity.Schedule.StartDateTime,
        RegularQuota = entity.RegularQuota,
        DifficultQuota = entity.DifficultQuota
    };

    public EmployeeQuotas ToEntity()
    {
        throw new NotImplementedException();
    }
}