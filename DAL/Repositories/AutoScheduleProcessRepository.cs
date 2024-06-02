using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Classes;

namespace SchedulerApi.DAL.Repositories;

public class AutoScheduleProcessRepository : Repository<AutoScheduleProcess>, IAutoScheduleProcessRepository
{


    public AutoScheduleProcessRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task<object> CreateAsync(AutoScheduleProcess entity)
    {
        var entityEntry = Context.AutoScheduleProcesses.Add(entity);
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }

    public override async Task<AutoScheduleProcess?> ReadAsync(object key)
    {
        if (key is not int id) throw new ArgumentException("key is not of expected type.");
        return await Context.AutoScheduleProcesses
            .Include(p => p.Desk)
            .ThenInclude(desk => desk.Unit)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<AutoScheduleProcess>> ReadAllAsync()
    {
        return await Context.AutoScheduleProcesses
            .Include(p => p.Desk)
            .ThenInclude(desk => desk.Unit)
            .ToListAsync();
    }

    public override async Task DeleteAsync(object key)
    {
        if (key is not int id)
        {
            throw new ArgumentException("key is not of expected type.");
        }
        
        var obj = await ReadAsync(id);
        if (obj is null)
        {
            return;
        }

        await DeleteAsync(obj);
    }

    public override async Task DeleteAsync(AutoScheduleProcess entity)
    {
        Context.AutoScheduleProcesses.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<AutoScheduleProcess?> ReadRunningAsync(string deskId, DateTime scheduleStart)
    {
        return await Context.AutoScheduleProcesses
            .Where(p => p.Status == TaskStatus.Running)
            .Where(p => p.DeskId == deskId)
            .Include(p => p.Desk)
            .ThenInclude(desk => desk.Unit)
            .FirstOrDefaultAsync(p => p.ScheduleStart == scheduleStart);
    }

    public override async Task UpdateAsync(AutoScheduleProcess entity)
    {
        Context.AutoScheduleProcesses.Update(entity);
        await Context.SaveChangesAsync();
    }
}
