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

    public override Task CreateAsync(Shift entity)
    {
        throw new NotImplementedException();
    }

    public override async Task<Shift?> ReadAsync(object key)
    {
        if (key is not DateTime shiftKey)
        {
            return null;
        }

        return await Context.Shifts.Include(shift => shift.Employee).FirstOrDefaultAsync(shift => shift.StartDateTime == shiftKey);
    }

    public override async Task<IEnumerable<Shift>> ReadAllAsync()
    {
        return await Context.Shifts.Include(shift => shift.Employee).ToListAsync();
    }

    public override Task DeleteAsync(object key)
    {
        throw new NotImplementedException();
    }

    public override Task DeleteAsync(Shift entity)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateShiftEmployeeAsync(DateTime shiftKey, Employee employee)
    {
        var shift = await Context.Shifts.FirstOrDefaultAsync(shift => shift.StartDateTime == shiftKey);
        if (shift == null) 
        {
            throw new KeyNotFoundException("Shift not found in database.");
        }
        shift.Employee = employee;
        // The Context.Shifts.Update(shift); line is removed, as it's unnecessary.
        await Context.SaveChangesAsync();
    }
}