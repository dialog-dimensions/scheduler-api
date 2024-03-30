using SchedulerApi.Models.Interfaces;
using SchedulerApi.Services.Workflows.Processes;

namespace SchedulerApi.Services.Workflows.Steps;

public interface IStep : IKeyProvider
{
    int Id { get; }
    Process? Process { get; set; }
    int ProcessId { get; }
    string? Name { get; }
    Func<object[], Task>? Task { get; set; }
    TaskStatus Status { get; }
    IStep? NextStep { get; set; }

    void Initialize(Func<object[], Task> task, IStep? nextStep = default);
    Task Run(object? parameter);
}