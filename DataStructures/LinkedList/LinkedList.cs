using System.Collections;

namespace SchedulerApi.DataStructures.LinkedList;

public class LinkedList<T> : IList<T>
{
    private readonly LinkedListNodeFactory<T> _nodeFactory = new();
    
    private LinkedListNode<T>? Head { get; set; }
    private LinkedListNode<T>? Tail { get; set; }
    
    public IEnumerator<T> GetEnumerator()
    {
        return new LinkedListEnumerator<T>(Head);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        var node = _nodeFactory.Create(item);
        
        if (Count == 0)
        {
            Head = node;
        }
        else
        {
            Tail!.SetNext(node);
        }

        Tail = node;
        Count++;
    }

    public void Clear()
    {
        Head = null;
        Tail = null;
        Count = 0;
    }

    public bool Contains(T item)
    {
        return Enumerable.Contains(this, item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        var enumIndex = 0;
        foreach (var e in this)
        {
            if (enumIndex >= arrayIndex)
            {
                array[arrayIndex - arrayIndex] = e;
            }

            enumIndex++;
        }
    }

    public bool Remove(T item)
    {
        var i = IndexOf(item);
        if (i == -1) return false;
        RemoveAt(i);
        return true;
    }

    public int Count { get; private set; }

    public bool IsReadOnly => false;
    public int IndexOf(T item)
    {
        var enumIndex = 0;
        foreach (var e in this)
        {
            if (e.Equals(item))
            {
                return enumIndex;
            }

            enumIndex++;
        }

        return -1;
    }

    public void Insert(int index, T item)
    {
        if (index == Count)
        {
            Add(item);
            return;
        }

        var node = _nodeFactory.Create(item);
        var nextNode = NodeAt(index);
        nextNode.SetPrevious(node);
        Count++;
    }

    public void RemoveAt(int index)
    {
        var node = NodeAt(index);
        if (node.Next is not null)
        {
            node.Next.SetPrevious(node.Previous);
        }
        if (node.Previous is not null)
        {
            node.Previous.SetNext(node.Next);
        }

        Count--;
    }

    private LinkedListNode<T> NodeAt(int index)
    {
        var t = Head;
        for (var i = 0; i < index; i++)
        {
            t = t.Next;
        }

        return t;
    }

    public T this[int index]
    {
        get => NodeAt(index).Value;
        set => NodeAt(index).Value = value;
    }
}
