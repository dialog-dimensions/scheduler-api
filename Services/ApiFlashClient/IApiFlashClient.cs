namespace SchedulerApi.Services.ApiFlashClient;

public interface IApiFlashClient
{
    Task<Stream> TakeScreenshotAsync(string url);
}