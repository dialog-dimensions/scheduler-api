using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Comparers.Interfaces;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine.Comparers;

public class EmployeeComparer : IEmployeeComparer
{
    public IAssignmentScorer Scorer { get; }

    private ScheduleData? Data { get; set; }
    private Shift? Shift { get; set; }
    public bool Initialized => Data is not null;
    public bool Ready => Initialized & Shift is not null;

    
    public EmployeeComparer(IAssignmentScorer scorer)
    {
        Scorer = scorer;
    }

    public void Initialize(ScheduleData data)
    {
        SetContext(data);
        Scorer.Initialize(data);
    }

    private void SetContext(ScheduleData data)
    {
        Data = data;
        Shift = default;
    }

    public void SetShift(DateTime shiftKey)
    {
        if (!Initialized) return;
        Shift = Data!.FindShift(shiftKey)!;
    }
    
    
    public int Compare(Employee? x, Employee? y)
    {
        if (!Ready) return 0;

        if (x is null | y is null)
        {
            return 0;
        }

        var xScore = Scorer.ScoreAssignment(Shift!.StartDateTime, x!.Id);
        var yScore = Scorer.ScoreAssignment(Shift.StartDateTime, y!.Id);

        return double.Sign(yScore - xScore);
    }
}
