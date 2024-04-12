using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.Organization;

public class Unit : IKeyProvider
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
}
