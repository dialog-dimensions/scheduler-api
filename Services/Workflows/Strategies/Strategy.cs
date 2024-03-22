using SchedulerApi.Services.Workflows.Steps;

namespace SchedulerApi.Services.Workflows.Strategies;

public class Strategy : IStrategy
{
    public IStep? InitialStep { get; private set; }
    private IStep? FinalStep { get; set; }
    public bool HasSteps => InitialStep is not null;
    
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
