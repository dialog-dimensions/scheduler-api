using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.Organization;

public class Desk : IKeyProvider
{
    public string Id { get; set; }
    public string Name { get; set; }

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
    public bool Active { get; set; }
    public object Key => Id;

    public ProcessParameters ProcessParameters { get; set; }
}
