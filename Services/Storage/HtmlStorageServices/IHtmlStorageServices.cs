using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.Storage.BlobStorageServices;

namespace SchedulerApi.Services.Storage.HtmlStorageServices;

public interface IHtmlStorageServices
{
    public static string GetBlobName(Schedule schedule, Employee employee) =>
        $"{IBlobStorageServices.GetBlobName(schedule, employee)}.html";

    public Task<string> StoreAsync(string html, Schedule schedule, Employee employee);
}