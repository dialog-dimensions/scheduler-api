namespace SchedulerApi.DataStructures.LinkedList;

public class LinkedListNodeFactory<T>
{
    public LinkedListNode<T> Create(T value)
    {
        return new LinkedListNode<T> { Value = value };
    }
}
