using SchedulerApi.DataStructures.LinkedList;

namespace SchedulerApi.DataStructures.Tree;

public class TreeNode<T>
{
    public T Value { get; set; }
    public TreeNode<T>? ParentNode { get; set; }
    public IEnumerable<TreeNode<T>> ChildNodes { get; set; } = new LinkedList.LinkedList<TreeNode<T>>();
}