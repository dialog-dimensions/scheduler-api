using SchedulerApi.Models.Entities.Workers.BaseClasses;

namespace SchedulerApi.Models.Entities.Workers;

public class Admin : Worker
{
    public Admin()
    {
        Role = "Admin";
    }
}