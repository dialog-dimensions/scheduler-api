using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine;

public class QuotaCalculator : IQuotaCalculator
{

    public ScheduleQuotas GetQuotas(ScheduleData data)
    {
        var schedule = data.Schedule;
        var employees = data.Employees.ToList();
        
        var projectedQuota = ProjectedQuota(schedule, employees);
        var projectedDifficultQuota = ProjectedDifficultQuota(schedule, employees);
        
        var result = new ScheduleQuotas
        {
            Schedule = schedule, 
            ProjectedRegularQuota = projectedQuota, 
            ProjectedDifficultQuota = projectedDifficultQuota
        };
        
        result.AddRange(employees.Select(employee => new EmployeeQuotas
            {
                Schedule = schedule, 
                Employee = employee, 
                RegularQuota = projectedQuota - CalculateCorrection(employee), 
                DifficultQuota = projectedDifficultQuota - CalculateDifficultCorrection(employee)
            })
        );
        
        return result;
    }

    private static int CountShifts(Schedule schedule)
    {
        return schedule.Count;
    }

    private static int CountDifficultShifts(Schedule schedule)
    {
        return schedule.Count(shift => shift.IsDifficult);
    }

    private static int CountEmployees(IEnumerable<Employee> employees)
    {
        return employees.Count();
    }

    private static double ProjectedQuotaFormula(int numberOfShifts, int numberOfEmployees)
    {
        return numberOfShifts / (double)numberOfEmployees;
    }

    private static double ProjectedQuota(Schedule schedule, IEnumerable<Employee> employees)
    {
        var numberOfShifts = CountShifts(schedule);
        var numberOfEmployees = CountEmployees(employees);
        return ProjectedQuotaFormula(numberOfShifts, numberOfEmployees);
    }

    private static double ProjectedDifficultQuota(Schedule schedule, IEnumerable<Employee> employees)
    {
        var numberOfDifficultShifts = CountDifficultShifts(schedule);
        var numberOfEmployees = CountEmployees(employees);
        return ProjectedQuotaFormula(numberOfDifficultShifts, numberOfEmployees);
    }

    private static double CorrectionFormula(double balance) =>
        double.Sign(balance) * double.Min(double.Abs(balance), 1);

    private static double CalculateCorrection(Employee employee) => 
        CorrectionFormula(employee.Balance);

    private static double CalculateDifficultCorrection(Employee employee) => 
        CorrectionFormula(employee.DifficultBalance);
}
