using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Models.DTOs;

public class FlatShiftEmployeeDto : IDto<Shift, FlatShiftEmployeeDto>
{
    public DateTime ShiftKey { get; set; }
    public Employee? Employee { get; set; }

    public static FlatShiftEmployeeDto FromEntity(Shift entity) => new()
    {
        ShiftKey = entity.StartDateTime,
        Employee = entity.Employee
    };

    public Shift ToEntity()
    {
        throw new NotImplementedException();
    }
}
