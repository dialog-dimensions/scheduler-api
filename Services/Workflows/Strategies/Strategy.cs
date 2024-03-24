using SchedulerApi.Services.Workflows.Steps;

namespace SchedulerApi.Services.Workflows.Strategies;

public class Strategy : IStrategy
{
    public IStep? InitialStep { get; private set; }
    private IStep? FinalStep { get; set; }
    public bool HasSteps => InitialStep is not null;
    public IEnumerable<string> StepsInStrategyToString()
    {
        var result = new List<string>();
        var step = InitialStep;
        while (step is not null)
        {
            result.Add(step.Task.Method.Name);
            step = step.NextStep;
        }
    
        return result;
    }

    public void AddStep(Func<object[], Task> task)
    {
        var step = new Step(task);
        
        if (InitialStep is null)
        {
            InitialStep = step;
            FinalStep = step;
        }
        
        else
        {
            FinalStep!.NextStep = step;
            FinalStep = step;
        }
    }
}
