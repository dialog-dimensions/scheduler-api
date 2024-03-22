using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine;

public class Balancer : IBalancer
{
    private readonly IQuotaCalculator _calculator;

    public ScheduleData Data { get; set; }
    public bool Initialized { get; set; }
    public bool Ready => Initialized;
    
    private ScheduleQuotas _quotas;
    private ScheduleQuotas Quotas
    {
        get => _quotas;
        set
        {
            if (Initialized) return;
            _quotas = value;
        }
    }

    private Dictionary<Employee, double> Balances { get; set; }
    private Dictionary<Employee, double> DifficultBalances { get; set; }


    public Balancer(IQuotaCalculator calculator)
    {
        _calculator = calculator;
        Initialized = false;
    }

    public void Initialize(ScheduleData data)
    {
        SetContext(data);
        Quotas = _calculator.GetQuotas(data);
        Balances = Quotas.ToDictionary(eq => eq.Employee, eq => eq.RegularQuota);
        DifficultBalances = Quotas.ToDictionary(eq => eq.Employee, eq => eq.DifficultQuota);
        Initialized = true;
    }
    
    public void SetContext(ScheduleData data)
    {
        Data = data;
    }
    
    
    public double GetQuota(Employee employee)
    {
        return Quotas.GetQuota(employee);
    }
    
    public double GetDifficultQuota(Employee employee)
    {
        return Quotas.GetDifficultQuota(employee);
    }

    
    
    public double GetRemainingBalance(Employee employee)
    {
        return Balances[employee];
    }

    public double GetRemainingDifficultBalance(Employee employee)
    {
        return DifficultBalances[employee];
    }

    public Dictionary<Employee, double> GetBalanceIncrements()
    {
        return Balances.ToDictionary(
            kv => kv.Key, 
            kv => -kv.Value
            );
    }

    public Dictionary<Employee, double> GetDifficultBalanceIncrements()
    {
        return DifficultBalances.ToDictionary(
            kv => kv.Key, 
            kv => -kv.Value
            );
    }


    public void OnShiftAssigned(Shift shift, Employee employee)
    {
        Balances[employee] -= 1;
        if (shift.IsDifficult) DifficultBalances[employee] -= 1;
    }

    public void OnShiftIsUnAssigning(Shift shift)
    {
        var employee = shift.Employee!;
        Balances[employee] += 1;
        if (shift.IsDifficult) DifficultBalances[employee] += 1;
    }

    public bool IsExhausted(Employee employee)
    {
        return Balances[employee] < 1;
    }

    public bool IsDifficultExhausted(Employee employee)
    {
        return DifficultBalances[employee] < 1;
    }

    public bool IsSuperExhausted(Employee employee)
    {
        return Balances[employee] < 0;
    }

    public bool IsDifficultSuperExhausted(Employee employee)
    {
        return DifficultBalances[employee] < 0;
    }
}