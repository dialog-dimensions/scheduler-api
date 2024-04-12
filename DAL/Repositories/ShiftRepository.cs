using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.DAL.Repositories;

public class ShiftRepository : Repository<Shift>, IShiftRepository
{
    public ShiftRepository(ApiDbContext context) : base(context)
    {
    }

    public override Task CreateAsync(Shift entity)
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
}