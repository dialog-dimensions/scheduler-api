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

    public double ScoreAssignment(string deskId, DateTime shiftStartDateTime, int employeeId)
    {
        if (!Ready) return 0.0;
        
        var shift = Data!.FindShift(deskId, shiftStartDateTime)!;
        var employee = Data.FindEmployee(employeeId)!;
        var exception = Data.FindException(deskId, shiftStartDateTime, employeeId);
            
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
            var previousShiftStart = shiftStartDateTime.AddHours(-Data.Schedule.ShiftDuration);
            var previousShift = Data.FindShift(deskId, previousShiftStart);
        
            if (previousShift is null) return false;
        
            if (previousShift.Employee?.Equals(employee) ?? false)
            {
                return !CheckBothShiftsHaveOnPreferences(shiftStartDateTime, previousShiftStart);
            }
        
            return false;
        }
        
        bool CheckNextDoubleShifts()
        {
            var nextShiftStart = shiftStartDateTime.AddHours(Data.Schedule.ShiftDuration);
            var nextShift = Data.FindShift(deskId, nextShiftStart);

            if (nextShift is null) return false;

            if (nextShift.Employee?.Equals(employee) ?? false)
            {
                return !CheckBothShiftsHaveOnPreferences(shiftStartDateTime, nextShiftStart);
            }

            return false;
        }
        
        bool CheckBothShiftsHaveOnPreferences(DateTime shiftStart1, DateTime shiftStart2)
        {
            var exception1 = Data.FindException(deskId, shiftStart1, employeeId);
            var exception2 = Data.FindException(deskId, shiftStart2, employeeId);

            return exception1 is { ExceptionType: ExceptionType.OnPreference } &
                   exception2 is { ExceptionType: ExceptionType.OnPreference };
        }
    }
}
