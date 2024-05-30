namespace SchedulerApi.Services.ImageGenerationServices.HtmlToImage;

public interface IHtmlImageGenerator
{
    Task<Stream> GenerateAsync(string html);
}