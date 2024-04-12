using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.DTOs.OrganizationEntities;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Enums;

namespace SchedulerApi.Models.DTOs;

public class ShiftSwapDto : IDto<ShiftSwap, ShiftSwapDto>
{
    public int SwapId { get; set; }
    public DeskDto Desk { get; set; }
    public DateTime ShiftStart { get; set; }
    public int PreviousEmployeeId { get; set; }
    public SwapStatus Status { get; set; }

    public static ShiftSwapDto FromEntity(ShiftSwap entity) => new()
    {
        SwapId = entity.SwapId,
        Desk = DeskDto.FromEntity(entity.Desk),
        ShiftStart = entity.ShiftStart,
        PreviousEmployeeId = entity.PreviousEmployeeId,
        Status = entity.Status
    };

    public ShiftSwap ToEntity() => new()
    {
        SwapId = SwapId,
        Desk = Desk.ToEntity(),
        ShiftStart = ShiftStart,
        PreviousEmployeeId = PreviousEmployeeId,
        Status = Status
    };
}
