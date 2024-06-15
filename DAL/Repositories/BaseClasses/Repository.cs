using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.DAL.Repositories.BaseClasses;

public abstract class Repository<T> : IRepository<T> where T : class, IKeyProvider
{
    protected readonly ApiDbContext Context;

    protected Repository(ApiDbContext context)
    {
        Context = context;
    }

    public virtual async Task<object> CreateAsync(T entity)
    {
        var set = Context.Set<T>();
        var entityEntry = set.Add(entity); 
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }
    
    public virtual async Task<T?> ReadAsync(object?[]? keys)
    {
        var set = Context.Set<T>(); 
        return await set.FindAsync(keys);
    }

    public virtual async Task<T?> ReadAsync(object key)
    {
        var set = Context.Set<T>();
        return await set.FindAsync(key);
    }

    public virtual async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }
    
    public virtual async Task UpdateAsync(T entity)
    {
        var set = Context.Set<T>();
        set.Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(object key)
    {
        var entity = await ReadAsync(key);
        if (entity is null)
        {
            return;
        }

        await DeleteAsync(entity);
    }

    public virtual async Task DeleteAsync(T entity)
    {
        var set = Context.Set<T>();
        set.Remove(entity);
        await Context.SaveChangesAsync();
    }
}
