using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleHtmlServices;

public interface IScheduleHtmlGenerator
{
    string Generate(Schedule schedule);
    string Generate(Schedule schedule, Employee employee);
}
