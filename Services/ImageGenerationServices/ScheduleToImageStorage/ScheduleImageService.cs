using SchedulerApi.Services.ImageGenerationServices.ScheduleToImage;
using SchedulerApi.Services.Storage;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleToImageStorage;

public class ScheduleImageService : IScheduleImageService
{
    public ScheduleImageService(IScheduleImageGenerator imageGenerator, IBlobStorageServices storageServices)
    {
        ImageGenerator = imageGenerator;
        StorageServices = storageServices;
    }

    public IScheduleImageGenerator ImageGenerator { get; set; }
    public IBlobStorageServices StorageServices { get; set; }
}