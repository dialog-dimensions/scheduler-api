using SchedulerApi.DataStructures.Tree;

namespace SchedulerApi.Models.Organization;

public class Organization : Tree<Unit>
{
    public string Name => Root.Value.Name;
    public IEnumerable<Unit> Units => Members();
}
