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

    public override Task DeleteAsync(Shift entity)
    {
        throw new NotImplementedException();
    }

    public override async Task UpdateAsync(Shift entity)
    {
        Context.Shifts.Update(entity);
        await Context.SaveChangesAsync();
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
