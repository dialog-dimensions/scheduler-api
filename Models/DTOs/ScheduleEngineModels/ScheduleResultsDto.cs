using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Models.DTOs.ScheduleEngineModels;

public class ScheduleResultsDto : IDto<ScheduleResults, ScheduleResultsDto>
{
    public ScheduleDto CompleteSchedule { get; set; }
    public ScheduleReportDto Report { get; set; }

    public static ScheduleResultsDto FromEntity(ScheduleResults entity) => new()
    {
        CompleteSchedule = ScheduleDto.FromEntity(entity.CompleteSchedule),
        Report = ScheduleReportDto.FromEntity(entity.Report)
    };

    public ScheduleResults ToEntity() => new()
    {
        CompleteSchedule = CompleteSchedule.ToEntity(),
        Report = Report.ToEntity()
    };
}
