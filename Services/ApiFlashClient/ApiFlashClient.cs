using Azure.Security.KeyVault.Secrets;

namespace SchedulerApi.Services.ApiFlashClient;

public class ApiFlashClient : IApiFlashClient
{
    private readonly HttpClient _httpClient;
    private readonly SecretClient _secretClient;

    public const string ApiFlashEndpoint = "https://api.apiflash.com/v1/urltoimage";
    
    private string AccessKeySecretName { get; set; }

    public ApiFlashClient(
        HttpClient httpClient, 
        IConfiguration configuration, 
        SecretClient secretClient)
    {
        _httpClient = httpClient;
        _secretClient = secretClient;
        AccessKeySecretName = configuration["KeyVault:SecretNames:ApiFlash:AccessKey"]!;
    }

    public async Task<Stream> TakeScreenshotAsync(string url)
    {
        var parameters = System.Web.HttpUtility.ParseQueryString(string.Empty);
        parameters["access_key"] = await GetAccessKeyAsync();
        parameters["url"] = url;
        parameters["quality"] = "100";
        parameters["crop"] = "1480,0,440,650";

        var response = await _httpClient.GetAsync(ApiFlashEndpoint + "?" + parameters);
        return await response.Content.ReadAsStreamAsync();
    }

    private async Task<string> GetAccessKeyAsync() => 
        (await _secretClient.GetSecretAsync(AccessKeySecretName))
        .Value
        .Value;
}