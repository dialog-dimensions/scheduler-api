using SchedulerApi.Services.Workflows.Processes.Interfaces;

namespace SchedulerApi.Services.Workflows.Processes.Factories.Interfaces;

public interface IAutoScheduleProcessFactory
{
    IAutoScheduleProcess Create();
}