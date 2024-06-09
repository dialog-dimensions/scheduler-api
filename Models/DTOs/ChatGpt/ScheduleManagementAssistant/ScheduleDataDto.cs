using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Models.DTOs.ChatGpt.ScheduleManagementAssistant;

public class ScheduleDataDto : IDto<ScheduleData, ScheduleDataDto>
{
    public ScheduleDto Schedule { get; set; }
    public IEnumerable<EmployeeManagementAssistant.EmployeeDto> Employees { get; set; }
    public IEnumerable<ShiftExceptionDto> Exceptions { get; set; }

    public static ScheduleDataDto FromEntity(ScheduleData entity) => new()
    {
        Schedule = ScheduleDto.FromEntity(entity.Schedule),
        Employees = entity.Employees.Select(EmployeeManagementAssistant.EmployeeDto.FromEntity),
        Exceptions = entity.Exceptions.Select(ShiftExceptionDto.FromEntity)
    };

    public ScheduleData ToEntity()
    {
        throw new NotImplementedException();
    }
}