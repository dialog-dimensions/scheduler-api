using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ScheduleEngine.Interfaces;

public interface IShiftAssigner
{
    ScheduleData? Data { get; set; }
    bool Ready { get; }
    void Initialize(ScheduleData data);
    void Assign(DateTime shiftKey);
    void Assign(DateTime shiftKey, int employeeId);
    Employee? UnAssign(DateTime shiftKey);
}