using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleToHtmlTable;

public interface IScheduleHtmlTableGenerator
{
    string Generate(Schedule schedule);
    string Generate(Schedule schedule, Employee employee);
}
