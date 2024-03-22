using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ScheduleEngine.Interfaces;

public interface IBalancer
{
    ScheduleData Data { get; set; }

    void Initialize(ScheduleData data);

    bool Ready { get; }

    double GetQuota(Employee employee);

    double GetDifficultQuota(Employee employee);

    double GetRemainingBalance(Employee employee);

    double GetRemainingDifficultBalance(Employee employee);

    Dictionary<Employee, double> GetBalanceIncrements();

    Dictionary<Employee, double> GetDifficultBalanceIncrements();

    bool IsExhausted(Employee employee);

    bool IsDifficultExhausted(Employee employee);

    bool IsSuperExhausted(Employee employee);

    bool IsDifficultSuperExhausted(Employee employee);
    
    void OnShiftAssigned(Shift shift, Employee employee);

    void OnShiftIsUnAssigning(Shift shift);
}