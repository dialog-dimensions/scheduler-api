using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.Entities;

public class ShiftSwap : IKeyProvider
{
    public int SwapId { get; set; }

    private Desk _desk;

    public Desk Desk
    {
        get => _desk;
        set
        {
            _desk = value;
            DeskId = value.Id;
        }
    }
    public string DeskId { get; private set; }
    
    public DateTime ShiftStart { get; set; }
    public Shift Shift { get; set; }
    public int PreviousEmployeeId { get; set; }
    public Employee PreviousEmployee { get; set; }

    public object Key => SwapId;

    public SwapStatus Status { get; set; } = SwapStatus.Applied;

    public DateTime ModificationDateTime { get; set; }
    public User ModificationUser { get; set; }
}