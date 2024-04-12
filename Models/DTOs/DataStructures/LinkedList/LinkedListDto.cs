using SchedulerApi.Models.DTOs.Interfaces;

namespace SchedulerApi.Models.DTOs.DataStructures.LinkedList;

public class LinkedListDto<T, TDto> : IDto<SchedulerApi.DataStructures.LinkedList.LinkedList<T>, LinkedListDto<T, TDto>> where TDto : IDto<T, TDto>
{
    
    
    public static LinkedListDto<T, TDto> FromEntity(SchedulerApi.DataStructures.LinkedList.LinkedList<T> entity)
    {
        throw new NotImplementedException();
    }

    public SchedulerApi.DataStructures.LinkedList.LinkedList<T> ToEntity()
    {
        throw new NotImplementedException();
    }
}