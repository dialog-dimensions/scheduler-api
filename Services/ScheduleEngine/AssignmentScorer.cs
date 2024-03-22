using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine;

public class AssignmentScorer : IAssignmentScorer
{
    private readonly IConfigurationSection _weights;
    private readonly IBalancer _balancer;
    
    public ScheduleData? Data { get; set; }

    private bool Initialized => Data is not null;
    public bool Ready => Initialized;

    public AssignmentScorer(IConfiguration configuration, IBalancer balancer)
    {
        _weights = configuration.GetSection("ScheduleEngine:Weights");
        _balancer = balancer;
    }

    public void Initialize(ScheduleData data)
    {
        SetContext(data);
    }

    private void SetContext(ScheduleData data)
    {
        Data = data;
    }
    

    private double GetWeight(string parameter)
    {
        return _weights.GetValue<double>(parameter);
    }

    public double ScoreAssignment(DateTime shiftKey, int employeeId)
    {
        if (!Ready) return 0.0;
        
        var shift = Data!.FindShift(shiftKey)!;
        var employee = Data.FindEmployee(employeeId)!;
        var exception = Data.FindException(shiftKey, employeeId);
            
        var result = 0.0;

        if (_balancer.IsSuperExhausted(employee))
        {
            // plus because balance is negative.
            result += GetWeight("Exhaustion") * _balancer.GetRemainingBalance(employee);
        }

        if (_balancer.IsExhausted(employee))
        {
            result -= GetWeight("Exhaustion");
        }
        
        if (shift.IsDifficult)
        {
            if (_balancer.IsDifficultSuperExhausted(employee))
            {
                // plus because balance is negative.
                result += GetWeight("DifficultExhaustion") * _balancer.GetRemainingDifficultBalance(employee);
            }

            if (_balancer.IsDifficultExhausted(employee))
            {
                result -= GetWeight("DifficultExhaustion");
            }
        }

        if (exception is { ExceptionType: ExceptionType.Constraint })
        {
            result -= GetWeight("Constraints");
        }

        if (exception is { ExceptionType: ExceptionType.OffPreference })
        {
            result -= GetWeight("OffPreferences");
        }

        if (exception is { ExceptionType: ExceptionType.OnPreference })
        {
            result += GetWeight("OnPreferences");
        }

        if (CheckPreviousDoubleShifts())
        {
            result -= GetWeight("DoubleShifts");
        }

        if (CheckNextDoubleShifts())
        {
            result -= GetWeight("DoubleShifts");
        }
        
        return result;
        
        
        bool CheckPreviousDoubleShifts()
        {
            var previousShiftKey = shiftKey.AddHours(-Data.Schedule.ShiftDuration);
            var previousShift = Data.FindShift(previousShiftKey);
        
            if (previousShift is null) return false;
        
            if (previousShift.Employee?.Equals(employee) ?? false)
            {
                return !CheckBothShiftsHaveOnPreferences(shiftKey, previousShiftKey);
            }
        
            return false;
        }
        
        bool CheckNextDoubleShifts()
        {
            var nextShiftKey = shiftKey.AddHours(Data.Schedule.ShiftDuration);
            var nextShift = Data.FindShift(nextShiftKey);

            if (nextShift is null) return false;

            if (nextShift.Employee?.Equals(employee) ?? false)
            {
                return !CheckBothShiftsHaveOnPreferences(shiftKey, nextShiftKey);
            }

            return false;
        }
        
        bool CheckBothShiftsHaveOnPreferences(DateTime shiftKey1, DateTime shiftKey2)
        {
            var exception1 = Data.FindException(shiftKey1, employeeId);
            var exception2 = Data.FindException(shiftKey2, employeeId);

            return exception1 is { ExceptionType: ExceptionType.OnPreference } &
                   exception2 is { ExceptionType: ExceptionType.OnPreference };
        }
    }
}
