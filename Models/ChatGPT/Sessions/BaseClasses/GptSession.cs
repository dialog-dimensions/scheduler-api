using System.ComponentModel.DataAnnotations.Schema;
using SchedulerApi.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.ChatGPT.Sessions.BaseClasses;

public abstract class GptSession : IKeyProvider
{
    [NotMapped] public bool ChangesPending { get; set; }

    public abstract GptSessionType Type { get; set; }

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

    public string ThreadId { get; set; }

    public string CurrentAssistantId { get; set; }

    public object Key => ThreadId;

    public GptSessionState State { get; set; } = GptSessionState.NotCreated;
    
    [NotMapped]
    public IEnumerable<Message> Messages { get; set; } = [];
    
    [NotMapped]
    public Message LatestMessage => Messages.MaxBy(msg => msg.TimeStamp)!;
}