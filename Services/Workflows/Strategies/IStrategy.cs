using SchedulerApi.Services.Workflows.Steps;

namespace SchedulerApi.Services.Workflows.Strategies;

public interface IStrategy
{
    IStep? InitialStep { get; }
    void AddStep(Func<object[], Task> task);
    bool HasSteps { get; }
}
