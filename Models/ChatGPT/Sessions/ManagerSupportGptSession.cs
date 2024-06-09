using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Sessions.BaseClasses;

namespace SchedulerApi.Models.ChatGPT.Sessions;

public class ManagerSupportGptSession : GptSession
{
    public override GptSessionType Type { get; set; } = GptSessionType.ManagerSupport;
}