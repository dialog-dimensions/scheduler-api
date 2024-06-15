using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.DAL.Queries;

public interface IQueryService
{
    Task<IEnumerable<T>> Query<T>(Dictionary<string, object> parameters) where T : class, IMyQueryable;
    Task<IEnumerable<IKeyProvider>> Query(Type type, Dictionary<string, object> parameters);
}
