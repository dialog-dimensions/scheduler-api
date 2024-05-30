using SchedulerApi.Services.ImageGenerationServices.HtmlToImage;
using SchedulerApi.Services.ImageGenerationServices.ScheduleToHtmlTable;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleToImage;

public class ScheduleImageGenerator : IScheduleImageGenerator
{
    public ScheduleImageGenerator(IScheduleHtmlTableGenerator htmlGenerator, IHtmlImageGenerator imageGenerator)
    {
        HtmlGenerator = htmlGenerator;
        ImageGenerator = imageGenerator;
    }

    public IScheduleHtmlTableGenerator HtmlGenerator { get; set; }
    public IHtmlImageGenerator ImageGenerator { get; set; }
}