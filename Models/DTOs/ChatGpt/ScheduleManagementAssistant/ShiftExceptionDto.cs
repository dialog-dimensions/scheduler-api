using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.DTOs.ChatGpt.ScheduleManagementAssistant;

public class ShiftExceptionDto : IDto<ShiftException, ShiftExceptionDto>
{
    public DateTime ShiftStartDateTime { get; set; }
    public int EmployeeId { get; set; }
    public string ExceptionType { get; set; }
    public string Reason { get; set; }
    public DateTime ModificationDateTime { get; set; }
    public string ModificationUser { get; set; }
    public string DeskId { get; set; }

    public static ShiftExceptionDto FromEntity(ShiftException entity) => new()
    {
        ShiftStartDateTime = entity.ShiftStartDateTime,
        EmployeeId = entity.EmployeeId,
        ExceptionType = entity.ExceptionType.ToString(),
        Reason = entity.Reason ?? "",
        ModificationDateTime = entity.ModificationDateTime,
        ModificationUser = entity.ModificationUser.ToString(),
        DeskId = entity.DeskId
    };

    public ShiftException ToEntity()
    {
        throw new NotImplementedException();
    }
}