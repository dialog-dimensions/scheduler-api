using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.Storage.BlobStorageServices;

namespace SchedulerApi.Services.Storage.HtmlStorageServices;

public class HtmlStorageServices : IHtmlStorageServices
{
    private readonly IBlobStorageServices _blobStorage;
    private string ContainerName { get; }

    public HtmlStorageServices(IBlobStorageServices blobStorage, IConfiguration configuration)
    {
        _blobStorage = blobStorage;
        ContainerName = configuration["AzureBlobStorage:ContainerNames:Html"]!;
    }

    public async Task<string> StoreAsync(string html, Schedule schedule, Employee employee) => 
        await _blobStorage.StoreAsync(
            html,
            ContainerName,
            IHtmlStorageServices.GetBlobName(schedule, employee),
            contentType: "text/html; charset=utf-8");
}