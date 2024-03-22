using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Models.DTOs.ScheduleEngineModels;

public class ScheduleQuotasDto : List<EmployeeQuotasDto>, IDto<ScheduleQuotas, ScheduleQuotasDto>
{
    public DateTime ScheduleKey { get; set; }

    public static ScheduleQuotasDto FromEntity(ScheduleQuotas entity)
    {
        var result = new ScheduleQuotasDto { ScheduleKey = entity.Schedule.StartDateTime };
        result.AddRange(entity.Select(EmployeeQuotasDto.FromEntity));
        return result;
    }

    public ScheduleQuotas ToEntity()
    {
        throw new NotImplementedException();
    }
}