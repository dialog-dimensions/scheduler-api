using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Models.DTOs;

public class EmployeeDto : IDto<Employee, EmployeeDto>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Balance { get; set; }
    public double DifficultBalance { get; set; }
    public bool Active { get; set; }
    public string Role { get; set; } = "Employee";

    public static EmployeeDto FromEntity(Employee entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Balance = entity.Balance,
        DifficultBalance = entity.DifficultBalance,
        Active = entity.Active,
        Role = entity.Role
    };

    public Employee ToEntity() => new()
    {
        Id = Id,
        Name = Name,
        Balance = Balance,
        DifficultBalance = DifficultBalance,
        Active = Active,
        Role = Role
    };
}