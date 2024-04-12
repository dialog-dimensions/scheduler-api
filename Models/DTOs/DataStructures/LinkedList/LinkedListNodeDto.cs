using SchedulerApi.DataStructures.LinkedList;
using SchedulerApi.Models.DTOs.Interfaces;

namespace SchedulerApi.Models.DTOs.DataStructures.LinkedList;

public class LinkedListNodeDto<T, TDto> : IDto<SchedulerApi.DataStructures.LinkedList.LinkedListNode<T>, LinkedListNodeDto<T, TDto>> where TDto : IDto<T, TDto>
{
    public TDto Value { get; set; }
    public LinkedListNodeDto<T, TDto>? Previous { get; set; }
    public LinkedListNodeDto<T, TDto>? Next { get; set; }

    public static LinkedListNodeDto<T, TDto>
        FromEntity(SchedulerApi.DataStructures.LinkedList.LinkedListNode<T> entity) => new()
    {
        Value = TDto.FromEntity(entity.Value),
        Previous = entity.Previous is null ? default : FromEntity(entity.Previous),
        Next = entity.Next is null ? default : FromEntity(entity.Next)
    };

    public SchedulerApi.DataStructures.LinkedList.LinkedListNode<T> ToEntity()
    {
        var result = new SchedulerApi.DataStructures.LinkedList.LinkedListNode<T> { Value = Value.ToEntity() };
        result.SetPrevious(Previous?.ToEntity());
        result.SetNext(Next?.ToEntity());

        return result;
    }
}
