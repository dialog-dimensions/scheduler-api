using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.DTOs.ChatGpt.EmployeeManagementAssistant;

public class DeskAssignmentDto : IDto<DeskAssignment, DeskAssignmentDto>
{
    public string DeskId { get; set; }
    public int EmployeeId { get; set; }

    public static DeskAssignmentDto FromEntity(DeskAssignment entity) => new()
    {
        DeskId = entity.DeskId,
        EmployeeId = entity.EmployeeId
    };

    public DeskAssignment ToEntity()
    {
        throw new NotImplementedException();
    }
}