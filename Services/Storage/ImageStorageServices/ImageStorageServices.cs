using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.Storage.BlobStorageServices;

namespace SchedulerApi.Services.Storage.ImageStorageServices;

public class ImageStorageServices : IImageStorageServices
{
    private readonly IBlobStorageServices _blobStorageServices;

    public string ContainerName { get; }
    
    public ImageStorageServices(IBlobStorageServices blobStorageServices, IConfiguration configuration)
    {
        _blobStorageServices = blobStorageServices;
        ContainerName = configuration["AzureBlobStorage:ContainerNames:Images"]!;
    }

    public async Task<string> StoreAsync(Stream imageStream, Schedule schedule, Employee employee) =>
        await _blobStorageServices.StoreAsync(
            imageStream, 
            ContainerName,
            IImageStorageServices.GetBlobName(schedule, employee));
}