using SchedulerApi.Models.Entities.Workers.BaseClasses;

namespace SchedulerApi.Models.Entities.Workers;

public class Manager : Worker
{
    public Manager()
    {
        Role = "Manager";
    }
}