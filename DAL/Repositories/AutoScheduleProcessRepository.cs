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

    public override async Task CreateAsync(AutoScheduleProcess entity)
    {
        Context.AutoScheduleProcesses.Add(entity);
        await Context.SaveChangesAsync();
    }

    public override async Task<AutoScheduleProcess?> ReadAsync(object key)
    {
        if (key is not int id) return null;
        return await Context.AutoScheduleProcesses.FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<AutoScheduleProcess>> ReadAllAsync()
    {
        return await Context.AutoScheduleProcesses.ToListAsync();
    }

    public override async Task DeleteAsync(object key)
    {
        var obj = await ReadAsync(key);
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

    public async Task<AutoScheduleProcess?> ReadRunningAsync(DateTime scheduleStart)
    {
        return await Context.AutoScheduleProcesses
            .Where(p => p.Status == TaskStatus.Running)
            .FirstOrDefaultAsync(p => p.ScheduleStart == scheduleStart);
    }

    public override async Task UpdateAsync(AutoScheduleProcess entity)
    {
        Context.AutoScheduleProcesses.Update(entity);
        await Context.SaveChangesAsync();
    }
}
