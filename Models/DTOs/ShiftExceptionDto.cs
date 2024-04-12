using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.DTOs.OrganizationEntities;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Enums;

namespace SchedulerApi.Models.DTOs;

public class ShiftExceptionDto : IDto<ShiftException, ShiftExceptionDto>
{
    public DeskDto Desk { get; set; }
    public DateTime ShiftStartDateTime { get; set; }
    public int EmployeeId { get; set; }
    public ExceptionType ExceptionType { get; set; }
    public string? Reason { get; set; }

    public static ShiftExceptionDto FromEntity(ShiftException entity) => new()
    {
        Desk = DeskDto.FromEntity(entity.Desk),
        ShiftStartDateTime = entity.ShiftStartDateTime,
        EmployeeId = entity.EmployeeId,
        ExceptionType = entity.ExceptionType,
        Reason = entity.Reason
    };

    public ShiftException ToEntity() => new()
    {
        ShiftStartDateTime = ShiftStartDateTime,
        EmployeeId = EmployeeId,
        ExceptionType = ExceptionType,
        Reason = Reason
    };
}