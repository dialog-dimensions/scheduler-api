using SchedulerApi.Models.Interfaces;
using SchedulerApi.Models.Organization;
using Twilio.TwiML.Voice;

namespace SchedulerApi.Models.Entities;

public class Schedule : List<Shift>, IKeyProvider
{
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
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int ShiftDuration { get; set; }
    public object Key => new {DeskId, StartDateTime};

    public bool IsFullyScheduled => this.All(shift => shift.EmployeeId is > 0);
}