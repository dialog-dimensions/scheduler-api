using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.DAL.Queries;

public interface IMyQueryable : IKeyProvider
{
    public static abstract IEnumerable<string> QueryPropertyNames { get; }
    public static abstract Dictionary<string, Type> NavigationPropertyTypes { get; }
    Dictionary<string, object?> QueryProperties { get; }
    Dictionary<string, object?> NavigationPropertyKeys { get; }

    public string? GetQueryProperty(string queryPropertyName) => QueryProperties[queryPropertyName]!.ToString();
    public object? GetNavigationPropertyKey(string navigationPropertyName) => NavigationPropertyKeys[navigationPropertyName];
}
