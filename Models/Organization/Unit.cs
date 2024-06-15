using SchedulerApi.DAL.Queries;

namespace SchedulerApi.Models.Organization;

public class Unit : IMyQueryable, IEquatable<Unit>
{
    public string Id { get; set; } = "1";
    public string Name { get; set; } = "main";

    private Unit? _parentUnit;

    public Unit? ParentUnit
    {
        get => _parentUnit;
        set
        {
            _parentUnit = value;
            ParentUnitId = value?.Id;
        }
    }
    public string? ParentUnitId { get; private set; } 
    public object Key => Id;

    public bool Equals(Unit? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Unit)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static IEnumerable<string> QueryPropertyNames { get; } = new[] { "UnitId", "UnitName" };

    public Dictionary<string, object?> QueryProperties => new()
    {
        { "UnitId", Id },
        { "UnitName", Name }
    };

    public Dictionary<string, object?> NavigationPropertyKeys => new();
    public static Dictionary<string, Type> NavigationPropertyTypes { get; } = new();
}
