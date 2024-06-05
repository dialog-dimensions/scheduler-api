using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.Storage.BlobStorageServices;

namespace SchedulerApi.Services.Storage.ImageStorageServices;

public interface IImageStorageServices
{
    Task<string> StoreAsync(Stream imageStream, Schedule schedule, Employee employee);

    string ContainerName { get; }
    
    public static string GetBlobName(Schedule schedule, Employee employee) => 
        $"{IBlobStorageServices.GetBlobName(schedule, employee)}.jpg";
}
