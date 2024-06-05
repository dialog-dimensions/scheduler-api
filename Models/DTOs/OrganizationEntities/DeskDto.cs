using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.DTOs.OrganizationEntities;

public class DeskDto : IDto<Desk, DeskDto>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public UnitDto Unit { get; set; }
    public bool Active { get; set; }

    public string CatchRangeString { get; set; }
    public string FileWindowDurationString { get; set; }
    public string HeadsUpDurationString { get; set; }

    public static DeskDto FromEntity(Desk entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Unit = UnitDto.FromEntity(entity.Unit),
        Active = entity.Active,
        CatchRangeString = entity.ProcessParameters.CatchRangeString,
        FileWindowDurationString = entity.ProcessParameters.FileWindowDurationString,
        HeadsUpDurationString = entity.ProcessParameters.HeadsUpDurationString
    };

    public Desk ToEntity() => new()
    {
        Id = Id,
        Name = Name,
        Unit = Unit.ToEntity(),
        Active = Active,
        ProcessParameters = new ProcessParameters
        {
            CatchRange = TimeSpan.Parse(CatchRangeString),
            FileWindowDuration = TimeSpan.Parse(FileWindowDurationString),
            HeadsUpDuration = TimeSpan.Parse(HeadsUpDurationString)
        }
    };
}
