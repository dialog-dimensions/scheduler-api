using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Enums;

namespace SchedulerApi.Models.DTOs;

public class ShiftSwapDto : IDto<ShiftSwap, ShiftSwapDto>
{
    public int SwapId { get; set; }
    public DateTime ShiftKey { get; set; }
    public int PreviousEmployeeId { get; set; }
    public SwapStatus Status { get; set; }

    public static ShiftSwapDto FromEntity(ShiftSwap entity) => new()
    {
        SwapId = entity.SwapId,
        ShiftKey = entity.ShiftKey,
        PreviousEmployeeId = entity.PreviousEmployeeId,
        Status = entity.Status
    };

    public ShiftSwap ToEntity() => new()
    {
        SwapId = SwapId,
        ShiftKey = ShiftKey,
        PreviousEmployeeId = PreviousEmployeeId,
        Status = Status
    };
}