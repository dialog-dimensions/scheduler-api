using SchedulerApi.Enums;
using SchedulerApi.Models.Entities.Workers.Interfaces;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.Entities.Workers.BaseClasses;

public abstract class Worker : IWorker, IEquatable<Worker>
{
    public object Key => Id;
    public int Id { get; set; }
    public string Name { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    public string Role { get; set; }

    
    private Unit _unit;
    public Unit Unit
    {
        get => _unit;
        set
        {
            _unit = value;
            UnitId = value.Id;
        }
    }
    
    
    public string UnitId { get; private set; }

    public bool Equals(Worker other)
    {
        return Id == other.Id && Name == other.Name;
    }

    public bool Equals(IWorker? other)
    {
        return other is not null && Equals(other as object);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Worker)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name);
    }

    public static IEnumerable<string> QueryPropertyNames { get; } = new[]
    {
        "EmployeeId", 
        "EmployeeName", 
        "EmployeeGender", 
        "EmployeeRole"
    };
    public static Dictionary<string, Type> NavigationPropertyTypes { get; } = new() 
    { 
        { "Unit", typeof(Unit) }
    };
    public Dictionary<string, object?> QueryProperties => new()
    {
        { "EmployeeId", Id },
        { "EmployeeName", Name },
        { "EmployeeGender", Gender },
        { "EmployeeRole", Role }
    };
    public Dictionary<string, object?> NavigationPropertyKeys => new()
    {
        { "Unit", UnitId },
    };
}
