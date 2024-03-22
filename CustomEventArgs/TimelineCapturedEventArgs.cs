namespace SchedulerApi.CustomEventArgs;

public class TimelineCapturedEventArgs : EventArgs
{
    public DateTime ProcessStart { get; set; }
    public DateTime FileWindowEnd { get; set; }
    public DateTime ProcessEnd { get; set; }
}
