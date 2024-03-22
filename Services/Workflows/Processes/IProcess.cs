using SchedulerApi.Services.Workflows.Steps;
using SchedulerApi.Services.Workflows.Strategies;

namespace SchedulerApi.Services.Workflows.Processes;

public interface IProcess
{
    IStrategy Strategy { get; }
    TaskStatus Status { get; }
    IStep? CurrentStep { get; }
    Task Proceed(object? parameter);
}
