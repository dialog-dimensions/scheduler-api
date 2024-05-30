using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Services.Storage;

public interface IBlobStorageServices
{
    Task<string> StoreImageAsync(Stream imageStream, string containerName, string blobName);
    Task<string> StoreScheduleImageAsync(Stream imageStream, Schedule schedule);
    Task<string> StoreScheduleImageAsync(Stream imageStream, Schedule schedule, Employee employee);
}
