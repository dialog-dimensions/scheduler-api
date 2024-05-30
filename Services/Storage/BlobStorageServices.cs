using Azure.Storage.Blobs;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ImageGenerationServices.ScheduleToImageStorage;

namespace SchedulerApi.Services.Storage;

public class BlobStorageServices : IBlobStorageServices
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfigurationSection _blobParams;
    
    public BlobStorageServices(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _blobParams = configuration.GetSection("AzureBlobStorage");
    }
    
    public async Task<string> StoreImageAsync(Stream imageStream, string blobName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(imageStream, overwrite: true);
        return blobClient.Uri.ToString();
    }

    public async Task<string> StoreScheduleImageAsync(Stream imageStream, Schedule schedule)
    {
        var containerName = _blobParams["BlobContainerName"]!;
        var blobName = $"{schedule.Desk.Name}_{schedule.StartDateTime.ToString($"yyyy-MM-dd")}.jpg";
        return await StoreImageAsync(imageStream, blobName, containerName);
    }
    
    public async Task<string> StoreScheduleImageAsync(Stream imageStream, Schedule schedule, Employee employee)
    {
        var containerName = _blobParams["BlobContainerName"]!;
        var blobName = IScheduleImageService.GetBlobName(schedule, employee);
        return await StoreImageAsync(imageStream, blobName, containerName);
    }
}
