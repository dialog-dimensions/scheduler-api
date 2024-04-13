using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.DTOs.OrganizationEntities;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.DTOs;

public class ScheduleDto : List<ShiftDto>, IDto<Schedule, ScheduleDto>
{
    public DeskDto Desk => SomeShift.Desk;
    public DateTime StartDateTime => FirstShift.StartDateTime;
    public DateTime EndDateTime => LastShift.EndDateTime;
    public int ShiftDuration => (int)(Duration.TotalHours / Count);
    public bool IsFullyScheduled => this.All(shift => shift.Employee is not null);
    
    private ShiftDto FirstShift => this.MinBy(s => s.StartDateTime)!;
    private ShiftDto LastShift => this.MaxBy(s => s.StartDateTime)!;
    private ShiftDto SomeShift => this[0];
    private TimeSpan Duration => EndDateTime.Subtract(StartDateTime);
    
    
    public static ScheduleDto FromEntity(Schedule entity)
    {
        var result = new ScheduleDto();
        result.AddRange(entity.Select(ShiftDto.FromEntity));
        return result;
    }
    
    public Schedule ToEntity()
    {
        var result = new Schedule();
        
        result.AddRange(this.Select(shiftDto => new Shift
        {
            Desk = shiftDto.Desk.ToEntity(),
            StartDateTime = shiftDto.StartDateTime,
            EndDateTime = shiftDto.EndDateTime,
            ScheduleStartDateTime = StartDateTime,
            Employee = shiftDto.Employee?.ToEntity()
        }));

        return result;
    }
}
