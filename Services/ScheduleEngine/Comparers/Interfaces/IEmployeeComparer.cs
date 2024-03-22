using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine.Comparers.Interfaces;

public interface IEmployeeComparer : IComparer<Employee>
{
    IAssignmentScorer Scorer { get; }

    bool Initialized { get; }
    bool Ready { get; }

    void Initialize(ScheduleData data);
    void SetShift(DateTime shiftKey);
}
