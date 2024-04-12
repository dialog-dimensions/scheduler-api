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
        if (key is not (string deskId, DateTime shiftStart, int employeeId))
        {
            throw new AggregateException("composite key is not of expected type.");
        }

        return await Context.Exceptions
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .FirstOrDefaultAsync(
                ex =>
                ex.DeskId == deskId &&
                ex.ShiftStartDateTime == shiftStart &&
                ex.EmployeeId == employeeId
                );
    }

    public override async Task<IEnumerable<ShiftException>> ReadAllAsync()
    {
        return await Context.Exceptions
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .ToListAsync();
    }

    public override async Task DeleteAsync(object key)
    {
        if (key is not (string deskId, DateTime shiftStart, int employeeId))
        {
            throw new ArgumentException("composite key is not of expected type.");
        }

        var entity = await Context.Exceptions.FindAsync(deskId, shiftStart, employeeId);
        if (entity is null)
        {
            return;
        }

        Context.Exceptions.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public override async Task DeleteAsync(ShiftException entity)
    {
        if (!Context.Exceptions.Contains(entity))
        {
            return;
        }

        Context.Exceptions.Remove(entity);
        await Context.SaveChangesAsync();
    }
    
    public async Task<IEnumerable<ShiftException>> WhereAsync(string deskId, DateTime shiftStartDateTime)
    {
        var result = await Context.Exceptions
            .Where(ex => ex.DeskId == deskId)
            .Where(ex => ex.ShiftStartDateTime.Equals(shiftStartDateTime))
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .ToListAsync();
        return result;
    }

    public async Task<IEnumerable<ShiftException>> WhereAsync(int employeeId)
    {
        var result = await Context.Exceptions
            .Where(ex => ex.EmployeeId.Equals(employeeId))
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .ToListAsync();
        return result;
    }

    public async Task<IEnumerable<ShiftException>> GetScheduleExceptions(string deskId, DateTime scheduleStartDateTime)
    {
        var result = await Context.Exceptions
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Where(ex => ex.DeskId == deskId)
            .Where(ex => ex.Shift.ScheduleStartDateTime.Equals(scheduleStartDateTime))
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .ToListAsync();
        return result;
    }
}
