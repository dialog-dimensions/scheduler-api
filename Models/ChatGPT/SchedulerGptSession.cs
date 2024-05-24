using System.ComponentModel.DataAnnotations.Schema;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Enums;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.ChatGPT;

public class SchedulerGptSession : IKeyProvider
{
    [NotMapped]
    public bool ChangesPending { get; set; }

    public int EmployeeId { get; set; }
    
    private Employee? _employee;
    public Employee? Employee
    {
        get => _employee;
        set
        {
            _employee = value;
            EmployeeId = value?.Id ?? 0;
        }
    }

    public string DeskId { get; set; } = "";

    public DateTime ScheduleStartDateTime { get; set; } = DateTime.MinValue;

    private Schedule? _schedule;
    
    [NotMapped]
    public Schedule? Schedule
    {
        get => _schedule;
        set
        {
            _schedule = value;
            DeskId = value?.DeskId ?? "";
            ScheduleStartDateTime = value?.StartDateTime ?? DateTime.MinValue;
        }
    }
    
    public string ThreadId { get; set; } = "";

    private ShabtzanGptConversationState _conversationState;

    public ShabtzanGptConversationState ConversationState
    {
        get => _conversationState;
        set
        {
            _conversationState = value;
            ChangesPending = true;
        }
    }

    [NotMapped]
    public IEnumerable<Message> Messages { get; set; } = [];

    public object Key => ThreadId;

    [NotMapped]
    public Message LatestMessage => Messages.MaxBy(msg => msg.TimeStamp)!;
}