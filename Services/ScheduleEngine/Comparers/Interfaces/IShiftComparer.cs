using SchedulerApi.Models.Entities;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ScheduleEngine.Comparers.Interfaces;

public interface IShiftComparer : IComparer<Shift>
{
    void Initialize(ScheduleData data);
}