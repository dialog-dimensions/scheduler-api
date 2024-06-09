using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.DTOs.ChatGpt.EmployeeManagementAssistant;

public class EmployeeDto : IDto<Employee, EmployeeDto>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public string UnitId { get; set; }
    public double Balance { get; set; }
    public double DifficultBalance { get; set; }
    public bool Active { get; set; }
    public string Role { get; set; }

    public static EmployeeDto FromEntity(Employee entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Gender = entity.Gender.ToString(),
        UnitId = entity.UnitId,
        Balance = entity.Balance,
        DifficultBalance = entity.DifficultBalance,
        Active = entity.Active,
        Role = entity.Role
    };

    public Employee ToEntity()
    {
        throw new NotImplementedException();
    }
}
