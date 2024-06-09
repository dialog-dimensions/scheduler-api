using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.ScheduleEngine;

public class ScheduleData : IKeyProvider
{
    public Schedule Schedule { get; set; }
    public IEnumerable<Employee> Employees { get; set; }
    public IEnumerable<ShiftException> Exceptions { get; set; }

    
    public Shift? FindShift(string deskId, DateTime shiftStartDateTime) => 
        Schedule.FirstOrDefault(
            shift => 
            shift.Desk.Id == deskId &&
            shift.StartDateTime == shiftStartDateTime
            );
    
    public Employee? FindEmployee(int employeeId) => 
        Employees.FirstOrDefault(emp => emp.Id == employeeId);

    public ShiftException? FindException(string deskId, DateTime shiftStartDateTime, int employeeId) =>
        Exceptions.FirstOrDefault(
            ex => 
                ex.EmployeeId == employeeId && 
                ex.DeskId == deskId && 
                ex.ShiftStartDateTime == shiftStartDateTime
                );

    public object Key => new { Schedule.DeskId, Schedule.StartDateTime };
}