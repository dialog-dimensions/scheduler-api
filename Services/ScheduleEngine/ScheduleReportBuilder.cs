using SchedulerApi.Models.DTOs;
using SchedulerApi.Models.DTOs.ScheduleEngineModels;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine;

public class ScheduleReportBuilder : IScheduleReportBuilder
{
    private readonly IQuotaCalculator _calculator;
    private readonly IScheduleScorer _scorer;

    public ScheduleReportBuilder(IQuotaCalculator calculator, IScheduleScorer scorer)
    {
        _calculator = calculator;
        _scorer = scorer;
    }

    public ScheduleReport BuildReport(ScheduleData data)
    {
        var quotas = _calculator.GetQuotas(data);
        return new ScheduleReport
        {
            Quotas = ScheduleQuotasDto.FromEntity(quotas),
            Increments = GetIncrements(data, quotas),
            Violations = GetViolations(data),
            Score = _scorer.Score(data)
        };
    }

    private ScheduleQuotasDto GetQuotas(ScheduleData data)
    {
        var quotas = _calculator.GetQuotas(data);
        return ScheduleQuotasDto.FromEntity(quotas);
    }

    private IEnumerable<EmployeeIncrements> GetIncrements(ScheduleData data, ScheduleQuotas quotas)
    {
        var shiftCounts = data.Employees.ToDictionary(emp => emp.Id, _ => new int[2]);
        foreach (var shift in data.Schedule)
        {
            var employeeId = shift.EmployeeId!.Value;
            shiftCounts[employeeId][0] += 1;
            if (shift.IsDifficult) shiftCounts[employeeId][1] += 1;
        }
        return data.Employees.Select(employee => new EmployeeIncrements 
        {
            EmployeeId = employee.Id, 
            RegularIncrement = shiftCounts[employee.Id][0] - quotas.ProjectedRegularQuota,
            DifficultIncrement = shiftCounts[employee.Id][1]  - quotas.ProjectedDifficultQuota
        });
    }
    
    private static IEnumerable<ShiftExceptionDto> GetViolations(ScheduleData data) => 
        data.Exceptions
            .Where(ex => data.Schedule
                .First(shift => shift.StartDateTime == ex.ShiftKey).EmployeeId == ex.EmployeeId)
            .Select(ShiftExceptionDto.FromEntity);
}