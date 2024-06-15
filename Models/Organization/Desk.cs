using SchedulerApi.DAL.Queries;

namespace SchedulerApi.Models.Organization;

public class Desk : IMyQueryable
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

    public static IEnumerable<string> QueryPropertyNames { get; } = new[]
    {
        "DeskId", "DeskName", "DeskActive", "DeskCatchRange", "DeskFileWindowDuration", "DeskApprovalWindowDuration",
        "DeskHeadsUpDuration", "DeskProcessDuration"
    };

    public Dictionary<string, object?> QueryProperties => new()
    {
        { "DeskId", Id },
        { "DeskName", Name },
        { "DeskActive", Active },
        { "DeskCatchRange", ProcessParameters.CatchRange },
        { "DeskFileWindowDuration", ProcessParameters.FileWindowDuration },
        { "DeskApprovalWindowDuration", ProcessParameters.ApprovalWindowDuration },
        { "DeskHeadsUpDuration", ProcessParameters.HeadsUpDuration },
        { "DeskProcessDuration", ProcessParameters.ProcessDuration }
    };

    public Dictionary<string, object?> NavigationPropertyKeys => new()
    {
        { "Unit", UnitId }
    };

    public static Dictionary<string, Type> NavigationPropertyTypes { get; } = new()
    {
        { "Unit", typeof(Unit) }
    };
}
