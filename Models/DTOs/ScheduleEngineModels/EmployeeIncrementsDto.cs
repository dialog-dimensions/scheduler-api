using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Models.DTOs.ScheduleEngineModels;

public class EmployeeIncrementsDto : EmployeeIncrements, IDto<EmployeeIncrements, EmployeeIncrementsDto>
{
    public static EmployeeIncrementsDto FromEntity(EmployeeIncrements entity) => new()
    {
        EmployeeId = entity.EmployeeId,
        RegularIncrement = entity.RegularIncrement, 
        DifficultIncrement = entity.DifficultIncrement
    };

    public EmployeeIncrements ToEntity()
    {
        return this;
    }
}
