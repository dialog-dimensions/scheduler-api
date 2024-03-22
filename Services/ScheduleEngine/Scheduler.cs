using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Factories;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Comparers.Interfaces;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine;

public class Scheduler : IScheduler
{
    private readonly IDataGatherer _gatherer;
    private readonly IShiftComparer _shiftComparer;
    private readonly IShiftAssigner _assigner;
    private readonly IScheduleReportBuilder _reportBuilder;
    private readonly IAssignmentScorer _assignmentScorer;
    private readonly IScheduleFactory _factory;
    
    public ScheduleData? Data { get; set; }

    public Scheduler(IDataGatherer gatherer, IShiftComparer shiftComparer, IShiftAssigner assigner, 
        IScheduleReportBuilder reportBuilder, IAssignmentScorer assignmentScorer, IScheduleFactory factory)
    {
        _gatherer = gatherer;
        _shiftComparer = shiftComparer;
        _assigner = assigner;
        _reportBuilder = reportBuilder;
        _assignmentScorer = assignmentScorer;
        _factory = factory;
    }

    private async Task Initialize(Schedule schedule)
    {
        var data = await _gatherer.GatherDataAsync(schedule.StartDateTime);
        data.Schedule = _factory.Copy(schedule);
        SetContext(data);
        InitializeComponents();
    }

    private void SetContext(ScheduleData data)
    {
        Data = data;
    }

    private void InitializeComponents()
    {
        _shiftComparer.Initialize(Data!);
        _assigner.Initialize(Data!);
    }
    

    public async Task<ScheduleResults> RunAsync(Schedule schedule)
    {
        await Initialize(schedule);
        
        var orderedShifts = Data!.Schedule.OrderBy(s => s, _shiftComparer).ToList();
        foreach (var shift in orderedShifts)
        {
            _assigner.Assign(shift.StartDateTime);
        }

        Console.WriteLine("Assignments Phase 1:");
        foreach (var shift in Data.Schedule)
        {
            Console.WriteLine($"{shift.StartDateTime} - {shift.EmployeeId}");
        }
        
        OptimizeWithSwaps(orderedShifts);

        return new ScheduleResults
        {
            CompleteSchedule = Data.Schedule,
            Report = _reportBuilder.BuildReport(Data)
        };
    }

    private void OptimizeWithSwaps(IReadOnlyList<Shift> shifts) 
    {
        var orderedShifts = shifts.OrderBy(shift => shift.StartDateTime).ToList();
        _assignmentScorer.Initialize(Data!);
        
        var runAgain = true;
        while (runAgain)
        {
            Console.WriteLine("Again");
            runAgain = false;
            for (var i = 0; i < orderedShifts.Count - 1; i++)
            {
                var iKey = orderedShifts[i].StartDateTime;
                for (var j = i + 1; j < orderedShifts.Count; j++)
                {
                    var jKey = orderedShifts[j].StartDateTime;
                    if (CheckPossibleSwitch(iKey, jKey))
                    {
                        SwitchAssignments(iKey, jKey);
                        runAgain = true;
                        break;
                    }
                }
            }
        }
    }

    private bool CheckPossibleSwitch(DateTime iKey, DateTime jKey)
    {
        var iEmployee = _assigner.UnAssign(iKey);
        var jEmployee = _assigner.UnAssign(jKey);

        var iScore = _assignmentScorer.ScoreAssignment(iKey, iEmployee!.Id);
        var jScore = _assignmentScorer.ScoreAssignment(jKey, jEmployee!.Id);

        var ijScore = _assignmentScorer.ScoreAssignment(iKey, jEmployee.Id);
        var jiScore = _assignmentScorer.ScoreAssignment(jKey, iEmployee.Id);

        var result = ijScore + jiScore > iScore + jScore;

        _assigner.Assign(iKey, iEmployee.Id);
        _assigner.Assign(jKey, jEmployee.Id);

        return result;
    }

    private void SwitchAssignments(DateTime iKey, DateTime jKey)
    {
        var iEmployee = _assigner.UnAssign(iKey);
        var jEmployee = _assigner.UnAssign(jKey);
        Console.WriteLine($"really {iEmployee.Id} in {iKey} and {jEmployee.Id} in {jKey}");
        _assigner.Assign(iKey, jEmployee!.Id);
        _assigner.Assign(jKey, iEmployee!.Id);
    }
}
