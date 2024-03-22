using SchedulerApi.Models.Entities.Workers.Interfaces;

namespace SchedulerApi.Models.Entities.Workers.BaseClasses;

public abstract class Worker : IWorker
{
    public object Key => Id;
    public int Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }

    protected bool Equals(Worker other)
    {
        return Id == other.Id && Name == other.Name && Role == other.Role;
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
        return HashCode.Combine(Id, Name, Role);
    }
}