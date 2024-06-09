using System.ComponentModel.DataAnnotations.Schema;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Sessions.BaseClasses;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.ChatGPT.Sessions;

public class GathererGptSession : GptSession
{
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
    
    private ShabtzanGptConversationState _conversationState = ShabtzanGptConversationState.NotCreated;

    public ShabtzanGptConversationState ConversationState
    {
        get => _conversationState;
        set
        {
            _conversationState = value;
            ChangesPending = true;
        }
    }

    public override GptSessionType Type { get; set; } = GptSessionType.ExceptionGathering;
}