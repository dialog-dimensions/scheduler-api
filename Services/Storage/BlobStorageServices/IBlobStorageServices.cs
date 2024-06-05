using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Services.Storage.BlobStorageServices;

public interface IBlobStorageServices
{
    Task<string> StoreAsync(Stream stream, string containerName, string blobName, string contentType = "application/octet-stream");
    Task<string> StoreAsync(string stringContent, string containerName, string blobName, string contentType = "application/octet-stream");

    string GetBlobUrl(string containerName, string blobName);

    public static string GetBlobName(Schedule schedule, Employee employee) => 
        $"{schedule.DeskId}_{schedule.StartDateTime:yy-MM-dd}_{employee.Id}";
}
