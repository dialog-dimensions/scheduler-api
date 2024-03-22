using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.DTOs;

public class ScheduleDto : List<ShiftDto>, IDto<Schedule, ScheduleDto>
{
    public DateTime? StartDateTime => this.MinBy(s => s.StartDateTime)?.StartDateTime;
    public DateTime? EndDateTime => this.MaxBy(s => s.EndDateTime)?.EndDateTime;

    public int? ShiftDuration => StartDateTime.HasValue & EndDateTime.HasValue
        ? (int)double.Round(EndDateTime!.Value.Subtract(StartDateTime!.Value).TotalHours / Count)
        : null;

    public bool IsFullyScheduled => this.All(shift => shift.Employee is not null);
    
    public static ScheduleDto FromEntity(Schedule entity)
    {
        var result = new ScheduleDto();
        result.AddRange(entity.Select(ShiftDto.FromEntity));
        return result;
    }
    
    public Schedule ToEntity()
    {
        var result = new Schedule
        {
            StartDateTime = StartDateTime!.Value, 
            EndDateTime = EndDateTime!.Value, 
            ShiftDuration = ShiftDuration!.Value
        };
        
        result.AddRange(this.Select(shiftDto => new Shift
        {
            StartDateTime = shiftDto.StartDateTime,
            EndDateTime = shiftDto.EndDateTime,
            ScheduleKey = StartDateTime!.Value,
            Employee = shiftDto.Employee?.ToEntity()
        }));

        return result;
    }
}
