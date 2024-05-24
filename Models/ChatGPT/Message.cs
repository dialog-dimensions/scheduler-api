namespace SchedulerApi.Models.ChatGPT;

public class Message
{
    public string Role { get; set; }
    public string Content { get; set; }
    public int TimeStamp { get; set; }
}