using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.DAL.Repositories.BaseClasses;

public abstract class Repository<T> : IRepository<T> where T : IKeyProvider
{
    protected readonly ApiDbContext Context;

    protected Repository(ApiDbContext context)
    {
        Context = context;
    }

    public abstract Task<object> CreateAsync(T entity);
    public abstract Task<T?> ReadAsync(object key);
    public abstract Task<IEnumerable<T>> ReadAllAsync();
    
    public virtual Task UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }
    
    public abstract Task DeleteAsync(object key);
    public abstract Task DeleteAsync(T entity);
}