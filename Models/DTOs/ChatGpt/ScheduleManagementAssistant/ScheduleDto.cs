using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.DTOs.ChatGpt.ScheduleManagementAssistant;

public class ScheduleDto : List<ShiftDto>, IDto<Schedule, ScheduleDto>
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string DeskId { get; set; }
    public int ShiftDuration { get; set; }

    public static ScheduleDto FromEntity(Schedule entity)
    {
        var result = new ScheduleDto
        {
            StartDateTime = entity.StartDateTime,
            EndDateTime = entity.EndDateTime,
            DeskId = entity.DeskId,
            ShiftDuration = entity.ShiftDuration
        };

        result.AddRange(entity.Select(ShiftDto.FromEntity));
        return result;
    }

    public Schedule ToEntity()
    {
        throw new NotImplementedException();
    }
}