using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.Entities;

public class ShiftSwap : IKeyProvider
{
    public int SwapId { get; set; }
    public DateTime ShiftKey { get; set; }
    public Shift Shift { get; set; }
    public int PreviousEmployeeId { get; set; }
    public Employee PreviousEmployee { get; set; }

    public object Key => SwapId;

    public SwapStatus Status { get; set; } = SwapStatus.Applied;

    public DateTime ModificationDateTime { get; set; }
    public User ModificationUser { get; set; }
}