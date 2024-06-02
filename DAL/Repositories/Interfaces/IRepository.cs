using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IRepository<T> where T : IKeyProvider
{
    Task<object> CreateAsync(T entity);

    Task<T?> ReadAsync(object key);

    Task<IEnumerable<T>> ReadAllAsync();

    Task UpdateAsync(T entity);

    Task DeleteAsync(object key);

    Task DeleteAsync(T entity);
}