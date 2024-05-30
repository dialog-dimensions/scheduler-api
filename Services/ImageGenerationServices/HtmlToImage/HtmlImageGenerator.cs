using PuppeteerSharp;

namespace SchedulerApi.Services.ImageGenerationServices.HtmlToImage;

public class HtmlImageGenerator : IHtmlImageGenerator
{
    public async Task<Stream> GenerateAsync(string html)
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();
        
        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

        using var page = await browser.NewPageAsync();

        await page.SetViewportAsync(new ViewPortOptions { Width = 1000, Height = 200 });

        await page.SetContentAsync(html);

        return await page.ScreenshotStreamAsync(new ScreenshotOptions { FullPage = true });
    }
}
