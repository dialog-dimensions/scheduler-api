using SchedulerApi.Services.Workflows.Steps;
using SchedulerApi.Services.Workflows.Strategies;

namespace SchedulerApi.Services.Workflows.Processes;

public class Process : IProcess
{
    public IStrategy Strategy { get; }
    public TaskStatus Status { get; private set; } = TaskStatus.Created;

    
    private IStep? _currentStep;
    public IStep? CurrentStep
    {
        get => _currentStep;
        private set
        {
            _currentStep = value;
            if (value is null)
            {
                Status = TaskStatus.RanToCompletion;
                Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Process Completed.");
            }
        }
    }

    public Process(IStrategy strategy)
    {
        if (!strategy.HasSteps)
        {
            throw new ArgumentException("Empty strategy given to the process.");
        }
        
        Strategy = strategy;
        CurrentStep = Strategy.InitialStep!;
    }
    
    
    public async Task Proceed(object? parameter = default)
    {
        if (Status == TaskStatus.Created)
        {
            Status = TaskStatus.Running;
            Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Process Running.");
        }
        
        if (Status != TaskStatus.Running)
        {
            return;
        }
        
        try
        {
            await CurrentStep!.Run(parameter);
            CurrentStep = CurrentStep.NextStep;
        }
        catch
        {
            Status = TaskStatus.Faulted;
            Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Process Faulted.");
            throw;
        }
    }
}
