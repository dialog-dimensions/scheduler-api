using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleImageServices;

public interface IScheduleImagePublisher
{
    Task<string> PublishScheduleImage(Schedule schedule, Employee employee);
}
