using Microsoft.EntityFrameworkCore;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.Workflows.Processes.Classes;

namespace SchedulerApi.DAL.Queries;

public class QueryService : IQueryService
{
    private readonly ApiDbContext _context;

    public QueryService(ApiDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> Query<T>(Dictionary<string, object> parameters) where T : class, IMyQueryable
    {
        var queryable = _context.Set<T>().AsQueryable();

        foreach (var navigationProperty in T.NavigationPropertyTypes.Keys)
        {
            queryable = queryable.Include(navigationProperty);
        }

        var query = await queryable.ToListAsync();
        
        // Query Using Direct Properties
        foreach (var queryPropertyName in T.QueryPropertyNames)
        {
            if (!parameters.ContainsKey(queryPropertyName))
            {
                continue;
            }

            query = query.Where(entity => 
                entity.GetQueryProperty(queryPropertyName)! == parameters[queryPropertyName].ToString()).ToList();
        }
        
        // Recursive querying for navigation properties
        var directParameters = parameters.Where(kv => T.QueryPropertyNames.Contains(kv.Key));
        var otherParameters = parameters.Except(directParameters).ToDictionary();
        if (!otherParameters.Any())
        {
            return query;
        }
        
        foreach (var (navigationPropertyName, navigationPropertyType) in T.NavigationPropertyTypes)
        {
            var navMatches = await Query(navigationPropertyType, otherParameters);

            query = query
                .Where(entity => navMatches
                    .Select(match => match.Key)
                    .Contains(entity.GetNavigationPropertyKey(navigationPropertyName)))
                .ToList();
        }

        return query;
    }

    private async Task<IEnumerable<T>> TryFind<T>(object key) where T : class, IKeyProvider
    {
        return await _context.Set<T>().Where(obj => obj.Key.Equals(key)).ToListAsync();
    }

    public async Task<IEnumerable<IKeyProvider>> Query(Type type, Dictionary<string, object> parameters)
    {
        if (type == typeof(Employee))
        {
            return await Query<Employee>(parameters);
        }
        else if (type == typeof(Shift))
        {
            return await Query<Shift>(parameters);
        }
        else if (type == typeof(Schedule))
        {
            return await Query<Schedule>(parameters);
        }
        else if (type == typeof(ShiftException))
        {
            return await Query<ShiftException>(parameters);
        }
        else if (type == typeof(Desk))
        {
            return await Query<Desk>(parameters);
        }
        else if (type == typeof(DeskAssignment))
        {
            return await Query<DeskAssignment>(parameters);
        }
        else if (type == typeof(Unit))
        {
            return await Query<Unit>(parameters);
        }
        else if (type == typeof(AutoScheduleProcess))
        {
            return await TryFind<AutoScheduleProcess>((int)parameters["ProcessId"]);
        }
        
        throw new ArgumentException("Type not supported");
    }
}
