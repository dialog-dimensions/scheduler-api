using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ApiFlashClient;
using SchedulerApi.Services.ImageGenerationServices.ScheduleHtmlServices;
using SchedulerApi.Services.Storage.HtmlStorageServices;
using SchedulerApi.Services.Storage.ImageStorageServices;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleImageServices;

public class ScheduleImagePublisher : IScheduleImagePublisher
{
    private IScheduleHtmlGenerator _htmlGenerator;
    private readonly IHtmlStorageServices _htmlStorageServices;
    private readonly IApiFlashClient _apiFlashClient;
    private readonly IImageStorageServices _imageStorageServices;

    public ScheduleImagePublisher(
        IScheduleHtmlGenerator htmlGenerator,
        IHtmlStorageServices htmlStorageServices, 
        IApiFlashClient apiFlashClient, 
        IImageStorageServices imageStorageServices)
    {
        _htmlGenerator = htmlGenerator;
        _htmlStorageServices = htmlStorageServices;
        _apiFlashClient = apiFlashClient;
        _imageStorageServices = imageStorageServices;
    }

    public async Task<string> PublishScheduleImage(Schedule schedule, Employee employee)
    {
        // Generate the HTML Code for the Schedule with Employee Shift Marks.
        var html = _htmlGenerator.Generate(schedule, employee);

        // Publish the HTML.
        var htmlUrl = await _htmlStorageServices.StoreAsync(html, schedule, employee);

        // Create a Screenshot of the HTML Page.
        var imageStream = await _apiFlashClient.TakeScreenshotAsync(htmlUrl);

        // Publish the Screenshot and Return Its URL.
        return await _imageStorageServices.StoreAsync(imageStream, schedule, employee);
    }
}
