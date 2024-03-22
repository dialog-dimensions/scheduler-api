namespace SchedulerApi.Services.Workflows.Steps;

public interface IStep
{
    string Name { get; }

    Func<object[], Task> Task { get; }
    TaskStatus Status { get; }
    IStep? NextStep { get; set; }
    Task Run(object? parameter);
}