using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.DTOs;

// COLLAPSING the schedule.
public class FlatScheduleDto : IDto<Schedule, FlatScheduleDto>
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int ShiftDuration { get; set; }
    public bool IsFullyScheduled { get; set; }

    public static FlatScheduleDto FromEntity(Schedule entity) => new()
    {
        StartDateTime = entity.StartDateTime,
        EndDateTime = entity.EndDateTime,
        ShiftDuration = entity.ShiftDuration,
        IsFullyScheduled = entity.IsFullyScheduled
    };

    public Schedule ToEntity()
    {
        throw new NotImplementedException();
    }
}
