using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.DTOs.OrganizationEntities;

public class UnitDto : IDto<Unit, UnitDto>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public UnitDto? ParentUnit { get; set; }

    public static UnitDto FromEntity(Unit entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        ParentUnit = entity.ParentUnit is null ? default : FromEntity(entity.ParentUnit)
    };

    public Unit ToEntity() => new()
    {
        Id = Id,
        Name = Name,
        ParentUnit = ParentUnit?.ToEntity()
    };
}
