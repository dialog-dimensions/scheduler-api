using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerApi.Models.Organization;

public class ProcessParameters
{
    public string CatchRangeString { get; set; } = "4.00:00:00";

    [NotMapped]
    public TimeSpan CatchRange
    {
        get => TimeSpan.Parse(CatchRangeString);
        set => CatchRangeString = value.ToString();
    }


    public string FileWindowDurationString { get; set; } = "1.00:00:00";

    [NotMapped]
    public TimeSpan FileWindowDuration
    {
        get => TimeSpan.Parse(FileWindowDurationString);
        set => FileWindowDurationString = value.ToString();
    }


    public string HeadsUpDurationString { get; set; } = "2.12:00:00";

    [NotMapped]
    public TimeSpan HeadsUpDuration
    {
        get => TimeSpan.Parse(HeadsUpDurationString);
        set => HeadsUpDurationString = value.ToString();
    }
    

    [NotMapped] public TimeSpan ApprovalWindowDuration => ProcessDuration - FileWindowDuration;
    [NotMapped] public TimeSpan ProcessDuration => CatchRange - HeadsUpDuration;
}