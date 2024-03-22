namespace SchedulerApi.Models.DTOs.Interfaces;

public interface IDto<T, out TDto> where TDto : IDto<T, TDto>
{
    public static abstract TDto FromEntity(T entity);
    T ToEntity();
}
