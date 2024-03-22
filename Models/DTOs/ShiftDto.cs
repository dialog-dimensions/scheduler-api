using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.DTOs;

public class ShiftDto : IDto<Shift, ShiftDto>
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime ScheduleKey { get; set; }
    public EmployeeDto? Employee { get; set; }
    
    public static ShiftDto FromEntity(Shift entity) => new()
    { 
        StartDateTime = entity.StartDateTime, 
        EndDateTime = entity.EndDateTime, 
        ScheduleKey = entity.ScheduleKey,
        Employee = entity.Employee is null ? null : EmployeeDto.FromEntity(entity.Employee)
    };

    public Shift ToEntity() => new()
    {
        StartDateTime = StartDateTime,
        EndDateTime = EndDateTime,
        ScheduleKey = ScheduleKey,
        Employee = Employee?.ToEntity()
    };
}
