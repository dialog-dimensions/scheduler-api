using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Comparers.Interfaces;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine;

public class ShiftAssigner : IShiftAssigner
{
    private readonly IEmployeeComparer _employeeComparer;
    private readonly IBalancer _balancer;
    
    public ScheduleData? Data { get; set; }

    private bool Initialized => Data is not null;
    public bool Ready => Initialized;

    public ShiftAssigner(IEmployeeComparer employeeComparer, IBalancer balancer)
    {
        _employeeComparer = employeeComparer;
        _balancer = balancer;
    }

    public void Initialize(ScheduleData data)
    {
        SetContext(data);
        InitializeComponents();
    }
    
    private void InitializeComponents()
    {
        _employeeComparer.Initialize(Data!);
        _balancer.Initialize(Data!);
    }

    public void SetContext(ScheduleData data)
    {
        Data = data;
    }

    private Employee? PickAssignment(DateTime shiftStart)
    {
        if (!Ready) return default;
        _employeeComparer.SetShift(Data.Schedule.DeskId, shiftStart);
        return Data!.Employees.MinBy(emp => emp, _employeeComparer);
    }
    
    public void Assign(DateTime shiftKey)
    {
        var employee = PickAssignment(shiftKey)!;
        Assign(shiftKey, employee);
    }

    public void Assign(DateTime shiftKey, int employeeId)
    {
        var employee = Data!.FindEmployee(employeeId)!;
        Assign(shiftKey, employee);
    }

    private void Assign(DateTime shiftStart, Employee employee)
    {
        var shift = Data!.FindShift(Data.Schedule.DeskId, shiftStart)!;
        shift.Employee = employee;
        _balancer.OnShiftAssigned(shift, employee);
    }
    
    public Employee? UnAssign(DateTime shiftStart)
    {
        var shift = Data!.FindShift(Data.Schedule.DeskId, shiftStart)!;
        var employee = shift.Employee;
        if (employee is null)
        {
            return null;
        }
        
        _balancer.OnShiftIsUnAssigning(shift);
        shift.Employee = default;
        return employee;
    }
}
