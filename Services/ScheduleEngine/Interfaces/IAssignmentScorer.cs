using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ScheduleEngine.Interfaces;

public interface IAssignmentScorer
{
    ScheduleData? Data { get; set; }

    bool Ready { get; }

    void Initialize(ScheduleData data);
    
    double ScoreAssignment(DateTime shiftKey, int employeeId);
}