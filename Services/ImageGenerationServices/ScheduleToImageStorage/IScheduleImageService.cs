using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ImageGenerationServices.ScheduleToImage;
using SchedulerApi.Services.Storage;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleToImageStorage;

public interface IScheduleImageService
{
    protected IScheduleImageGenerator ImageGenerator { get; set; }
    protected IBlobStorageServices StorageServices { get; set; }

    public async Task<string> Run(Schedule schedule)
    {
        var stream = await ImageGenerator.GenerateAsync(schedule);
        return await StorageServices.StoreScheduleImageAsync(stream, schedule);
    }
    
    public async Task<string> Run(Schedule schedule, Employee employee)
    {
        var stream = await ImageGenerator.GenerateAsync(schedule, employee);
        return await StorageServices.StoreScheduleImageAsync(stream, schedule, employee);
    }

    public static string GetBlobName(Schedule schedule, Employee employee) => 
        $"{schedule.DeskId}_{schedule.StartDateTime:yy-MM-dd}_{employee.Id}.jpg";
}
