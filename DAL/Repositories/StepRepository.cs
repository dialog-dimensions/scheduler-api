using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Steps;

namespace SchedulerApi.DAL.Repositories;

public class StepRepository : Repository<Step>, IRepository<Step>
{
    public StepRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task<object> CreateAsync(Step entity)
    {
        var entityEntry = Context.Steps.Add(entity);
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }

    public override async Task<Step?> ReadAsync(object key)
    {
        if (key is not int id) return default;
        return await Context.Steps.FirstOrDefaultAsync(s => s.Id == id);
    }

    public override async Task<IEnumerable<Step>> ReadAllAsync()
    {
        return await Context.Steps.ToListAsync();
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

    public override async Task DeleteAsync(Step entity)
    {
        Context.Steps.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public override Task<IEnumerable<Step>> Query(Dictionary<string, object> parameters, string prefixDiscriminator = "")
    {
        throw new NotImplementedException();
    }

    public override async Task UpdateAsync(Step entity)
    {
        Context.Steps.Update(entity);
        await Context.SaveChangesAsync();
    }
}
