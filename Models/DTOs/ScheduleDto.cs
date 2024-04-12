using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.DTOs.OrganizationEntities;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.DTOs;

public class ScheduleDto : List<ShiftDto>, IDto<Schedule, ScheduleDto>
{
    public DeskDto Desk { get; set; }
    public DateTime? StartDateTime => this.MinBy(s => s.StartDateTime)?.StartDateTime;
    public DateTime? EndDateTime => this.MaxBy(s => s.EndDateTime)?.EndDateTime;

    public int? ShiftDuration => StartDateTime.HasValue & EndDateTime.HasValue
        ? (int)double.Round(EndDateTime!.Value.Subtract(StartDateTime!.Value).TotalHours / Count)
        : null;

    public bool IsFullyScheduled => this.All(shift => shift.Employee is not null);
    
    public static ScheduleDto FromEntity(Schedule entity)
    {
        var result = new ScheduleDto { Desk = DeskDto.FromEntity(entity.Desk) };
        result.AddRange(entity.Select(ShiftDto.FromEntity));
        return result;
    }
    
    public Schedule ToEntity()
    {
        var result = new Schedule
        {
            Desk = Desk.ToEntity(),
            StartDateTime = StartDateTime!.Value, 
            EndDateTime = EndDateTime!.Value, 
            ShiftDuration = ShiftDuration!.Value
        };
        
        result.AddRange(this.Select(shiftDto => new Shift
        {
            Desk = shiftDto.Desk.ToEntity(),
            StartDateTime = shiftDto.StartDateTime,
            EndDateTime = shiftDto.EndDateTime,
            ScheduleStartDateTime = StartDateTime!.Value,
            Employee = shiftDto.Employee?.ToEntity()
        }));

        return result;
    }
}
