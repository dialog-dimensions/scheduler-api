using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ImageGenerationServices.HtmlToImage;
using SchedulerApi.Services.ImageGenerationServices.ScheduleToHtmlTable;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleToImage;

public interface IScheduleImageGenerator
{
    protected IScheduleHtmlTableGenerator HtmlGenerator { get; set; }
    protected IHtmlImageGenerator ImageGenerator { get; set; }

    public async Task<Stream> GenerateAsync(Schedule schedule) => 
        await ImageGenerator.GenerateAsync(
            HtmlGenerator.Generate(schedule));
    
    public async Task<Stream> GenerateAsync(Schedule schedule, Employee employee) => 
        await ImageGenerator.GenerateAsync(
            HtmlGenerator.Generate(schedule, employee));
}
