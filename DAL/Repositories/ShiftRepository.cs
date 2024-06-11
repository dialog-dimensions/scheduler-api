using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.DAL.Repositories;

public class ShiftRepository : Repository<Shift>, IShiftRepository
{
    public ShiftRepository(ApiDbContext context) : base(context)
    {
    }

    public override Task<object> CreateAsync(Shift entity)
    {
        throw new NotImplementedException();
    }

    public override async Task<Shift?> ReadAsync(object key)
    {
        if (key is not (string deskId, DateTime shiftStart))
        {
            throw new ArgumentException("composite key is not of expected type.");
        }

        return await Context.Shifts
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Where(shift => shift.DeskId == deskId)
            .FirstOrDefaultAsync(shift => shift.StartDateTime == shiftStart);
    }

    public override async Task<IEnumerable<Shift>> ReadAllAsync()
    {
        return await Context.Shifts
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .ToListAsync();
    }

    public override Task DeleteAsync(object key)
    {
        throw new NotImplementedException();
    }

    public override Task DeleteAsync(Shift entity)
    {
        throw new NotImplementedException();
    }

    public override async Task<IEnumerable<Shift>> Query(Dictionary<string, object> parameters, string prefixDiscriminator = "")
    {
        var hasShiftStartDateTime = parameters.TryGetValue("ShiftStartDateTime", out var shiftStartDateTimeValue);
        var hasDeskId = parameters.TryGetValue("DeskId", out var deskIdValue);

        if (hasShiftStartDateTime && hasDeskId)
        {
            var deskId = Convert.ToString(deskIdValue);
            var shiftStartDateTime = Convert.ToDateTime(shiftStartDateTimeValue);

            var shift = await ReadAsync((deskId, shiftStartDateTime));
            if (shift is null)
            {
                return new Shift[] { };
            }

            return new[] { shift };
        }

        var matches = Context.Shifts.AsQueryable();

        if (hasDeskId)
        {
            var deskId = Convert.ToString(deskIdValue);
            matches = matches.Where(shift => shift.DeskId == deskId);
        }

        if (hasShiftStartDateTime)
        {
            var shiftStartDateTime = Convert.ToDateTime(shiftStartDateTimeValue);
            matches = matches.Where(shift => shift.StartDateTime == shiftStartDateTime);
        }

        var hasShiftEndDateTime = parameters.TryGetValue("ShiftEndDateTime", out var shiftEndDateTimeValue);
        if (hasShiftEndDateTime)
        {
            var shiftEndDateTime = Convert.ToDateTime(shiftEndDateTimeValue);
            matches = matches.Where(shift => shift.EndDateTime == shiftEndDateTime);
        }
        
        var hasScheduleStartDateTime =
            parameters.TryGetValue("ScheduleStartDateTime", out var scheduleStartDateTimeValue);
        if (hasScheduleStartDateTime)
        {
            var scheduleStartDateTime = Convert.ToDateTime(scheduleStartDateTimeValue);
            matches = matches.Where(shift => shift.ScheduleStartDateTime == scheduleStartDateTime);
        }
        
        var hasEmployeeId = parameters.TryGetValue("EmployeeId", out var employeeIdValue);
        if (hasEmployeeId)
        {
            var employeeId = Convert.ToInt32(employeeIdValue);
            matches = matches.Where(shift => shift.EmployeeId == employeeId);
        }

        return await matches.ToListAsync();
    }

    public async Task UpdateShiftEmployeeAsync(string deskId, DateTime shiftStart, Employee employee)
    {
        var shift = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(shift => shift.DeskId == deskId)
            .FirstOrDefaultAsync(shift => shift.StartDateTime == shiftStart);
        if (shift == null) 
        {
            throw new KeyNotFoundException("Shift not found in database.");
        }
        shift.Employee = employee;
        // The Context.Shifts.Update(shift); line is removed, as it's unnecessary.
        await Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Shift>> GetEmployeeShiftsByRange(int employeeId, DateTime from, DateTime to) => 
        await Context.Shifts
            .Where(s => s.EmployeeId == employeeId)
            .Where(s => s.StartDateTime >= from)
            .Where(s => s.StartDateTime <= to)
            .Include(s => s.Desk)
            .ThenInclude(d => d.Unit)
            .Include(s => s.Employee)
            .ThenInclude(emp => emp.Unit)
            .ToListAsync();

    public async Task<IEnumerable<Shift>> GetDeskShiftsByRange(string deskId, DateTime from, DateTime to) =>
        await Context.Shifts
            .Where(s => s.DeskId == deskId)
            .Where(s => s.StartDateTime >= from)
            .Where(s => s.StartDateTime < to)
            .Include(s => s.Desk)
            .ThenInclude(d => d.Unit)
            .Include(s => s.Employee)
            .ThenInclude(e => e.Unit)
            .ToListAsync();

    public async Task<IEnumerable<Shift>> GetEmployeeShifts(int employeeId) =>
        await Context.Shifts
            .Where(s => s.EmployeeId == employeeId)
            .Include(s => s.Desk)
            .ThenInclude(d => d.Unit)
            .Include(s => s.Employee)
            .ThenInclude(e => e.Unit)
            .ToListAsync();

    public async Task<IEnumerable<Shift>> GetDeskShifts(string deskId) =>
        await Context.Shifts
            .Where(s => s.DeskId == deskId)
            .Include(s => s.Desk)
            .ThenInclude(d => d.Unit)
            .Include(s => s.Employee)
            .ThenInclude(e => e.Unit)
            .ToListAsync();
}
