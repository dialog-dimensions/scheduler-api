using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.DTOs.OrganizationEntities;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Models.DTOs;

public class FlatShiftEmployeeDto : IDto<Shift, FlatShiftEmployeeDto>
{
    public DeskDto Desk { get; set; }
    public DateTime ShiftStart { get; set; }
    public Employee? Employee { get; set; }

    public static FlatShiftEmployeeDto FromEntity(Shift entity) => new()
    {
        Desk = DeskDto.FromEntity(entity.Desk),
        ShiftStart = entity.StartDateTime,
        Employee = entity.Employee
    };

    public Shift ToEntity()
    {
        throw new NotImplementedException();
    }
}
