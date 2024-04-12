namespace SchedulerApi.DataStructures.Tree;

public class Tree<T>
{
    public TreeNode<T> Root { get; set; }

    protected IEnumerable<T> Members()
    {
        var result = new List<T> { Root.Value };
        result.AddRange(
            Root.ChildNodes
                .SelectMany(node => new Tree<T> {Root = node}
                    .Members()
                )
            );
        return result;
    }
}
