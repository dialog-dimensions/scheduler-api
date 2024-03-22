using System.Reflection;

namespace SchedulerApi.Services.Workflows.Steps;

public class Step : IStep
{
    public string Name => Task.GetMethodInfo().Name;
    public Func<object[], Task> Task { get; }
    public IStep? NextStep { get; set; }
    public TaskStatus Status { get; private set; } = TaskStatus.Created;
    
    public Step(Func<object[], Task> task, IStep? nextStep = default)
    {
        Task = task;
        NextStep = nextStep;
    }
    
    public async Task Run(object? parameter)
    {
        try
        {
            Status = TaskStatus.Running;
            Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Running Step: {Name}");
            await Task.Invoke(parameter is null ? Array.Empty<object>() : (object[])parameter);
            Status = TaskStatus.RanToCompletion;
            Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Step Completed!");
        }
        catch
        {
            Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Step Faulted");
            Status = TaskStatus.Faulted;
            throw;
        }
    }
}
