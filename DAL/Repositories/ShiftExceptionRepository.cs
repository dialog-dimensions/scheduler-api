using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.DAL.Repositories;

public class ShiftExceptionRepository : Repository<ShiftException>, IShiftExceptionRepository
{
    public ShiftExceptionRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task CreateAsync(ShiftException entity)
    {
        Context.Exceptions.Add(entity);
        await Context.SaveChangesAsync();
    }

    public override async Task<ShiftException?> ReadAsync(object key)
    {
        return await Context.Exceptions.FirstOrDefaultAsync(ex => ex.Key.Equals(key));
    }

    public override async Task<IEnumerable<ShiftException>> ReadAllAsync()
    {
        return await Context.Exceptions.ToListAsync();
    }

    public override async Task DeleteAsync(object key)
    {
        var entity = await Context.Exceptions.FirstOrDefaultAsync(ex => ex.Key.Equals(key));
        if (entity is null)
        {
            throw new KeyNotFoundException("Exception not found in database.");
        }

        Context.Exceptions.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public override async Task DeleteAsync(ShiftException entity)
    {
        if (!Context.Exceptions.Contains(entity))
        {
            throw new KeyNotFoundException("Exception not found in database.");
        }

        Context.Exceptions.Remove(entity);
        await Context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<ShiftException>> WhereAsync(DateTime shiftKey)
    {
        var result = await Context.Exceptions
            .Where(ex => ex.ShiftKey.Equals(shiftKey))
            .ToListAsync();
        return result;
    }

    public async Task<IEnumerable<ShiftException>> WhereAsync(int employeeId)
    {
        var result = await Context.Exceptions
            .Where(ex => ex.EmployeeId.Equals(employeeId))
            .ToListAsync();
        return result;
    }

    public async Task<IEnumerable<ShiftException>> GetScheduleExceptions(DateTime scheduleKey)
    {
        var result = await Context.Exceptions
            .Include(ex => ex.Shift)
            .Where(ex => ex.Shift.ScheduleKey.Equals(scheduleKey))
            .ToListAsync();
        return result;
    }
}
