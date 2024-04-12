namespace SchedulerApi.DataStructures.LinkedList;

public class LinkedListNode<T>
{
    public T Value { get; set; }

    private LinkedListNode<T>? _previous;
    public LinkedListNode<T>? Previous
    {
        get => _previous;
        private set
        {
            if (!_previous?.Equals(value) ?? true)
            {
                _previous = value;
            }
        }
    }

    private LinkedListNode<T>? _next;
    public LinkedListNode<T>? Next
    {
        get => _next;
        private set
        {
            if (!_next?.Equals(value) ?? true)
            {
                _next = value;
            }
        }
    }

    public void SetPrevious(LinkedListNode<T>? node)
    {
        Previous = node;
        if (node is not null)
        {
            node.Next = this;
        }
    }
    
    public void SetNext(LinkedListNode<T>? node)
    {
        Next = node;
        if (node is not null)
        {
            node.Previous = this;
        }
    }
}
