using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.DTOs.OrganizationEntities;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.DTOs;

public class ShiftDto : IDto<Shift, ShiftDto>
{
    public DeskDto Desk { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime ScheduleStartDateTime { get; set; }
    public EmployeeDto? Employee { get; set; }
    
    public static ShiftDto FromEntity(Shift entity) => new()
    { 
        Desk = DeskDto.FromEntity(entity.Desk),
        StartDateTime = entity.StartDateTime, 
        EndDateTime = entity.EndDateTime, 
        ScheduleStartDateTime = entity.ScheduleStartDateTime,
        Employee = entity.Employee is null ? null : EmployeeDto.FromEntity(entity.Employee)
    };

    public Shift ToEntity() => new()
    {
        Desk = Desk.ToEntity(),
        StartDateTime = StartDateTime,
        EndDateTime = EndDateTime,
        ScheduleStartDateTime = ScheduleStartDateTime,
        Employee = Employee?.ToEntity()
    };
}
