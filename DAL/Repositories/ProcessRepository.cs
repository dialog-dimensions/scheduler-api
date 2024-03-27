using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes;

namespace SchedulerApi.DAL.Repositories;

public class ProcessRepository : Repository<Process>, IProcessRepository
    
{
    public ProcessRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task CreateAsync(Process entity)
    {
        Context.Processes.Add(entity);
        await Context.SaveChangesAsync();
    }

    public override async Task<Process?> ReadAsync(object key)
    {
        return await Context.Processes.FindAsync(key);
    }

    public override async Task<IEnumerable<Process>> ReadAllAsync()
    {
        return await Context.Processes.ToListAsync();
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

    public override async Task DeleteAsync(Process entity)
    {
        Context.Processes.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Process>> ReadRunningAsync()
    {
        return await Context.Processes
            .Where(p => p.Status == TaskStatus.Created || p.Status == TaskStatus.Running)
            .ToListAsync();
    }
}