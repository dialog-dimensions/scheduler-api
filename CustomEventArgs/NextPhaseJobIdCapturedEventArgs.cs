namespace SchedulerApi.CustomEventArgs;

public class NextPhaseJobIdCapturedEventArgs : EventArgs
{
    public string JobId { get; set; }
}