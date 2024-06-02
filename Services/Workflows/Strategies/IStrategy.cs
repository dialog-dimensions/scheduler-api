using SchedulerApi.Services.Workflows.Steps;

namespace SchedulerApi.Services.Workflows.Strategies;

public interface IStrategy
{
    int ProcessId { get; set; }
    IStep? InitialStep { get; }
    void AddStep(Func<object[], Task> task);
    bool HasSteps { get; }

    IEnumerable<string> StepsInStrategyToString();
}
