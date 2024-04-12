using System.Collections;

namespace SchedulerApi.DataStructures.LinkedList;

public class LinkedListEnumerator<T> : IEnumerator<T>
{
    private LinkedListNode<T> HeadNode { get; }
    private LinkedListNode<T> CurrentNode { get; set; }

    public LinkedListEnumerator(LinkedListNode<T> headNode)
    {
        HeadNode = headNode;
        CurrentNode = headNode;
    }
    
    public bool MoveNext()
    {
        if (CurrentNode.Next is null) return false;
        CurrentNode = CurrentNode.Next;
        return true;
    }

    public void Reset()
    {
        CurrentNode = HeadNode;
    }

    public T Current => CurrentNode.Value;

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        
    }
}