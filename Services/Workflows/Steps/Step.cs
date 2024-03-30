using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes;

namespace SchedulerApi.Services.Workflows.Steps;

public class Step : IStep
{
    private readonly IRepository<Step> _repository;
    
    public int Id { get; set; }

    private Process? _process;
    public Process? Process
    {
        get=> _process;
        set
        {
            _process = value;
            ProcessId = value?.Id ?? 0;
        }
    }
    private bool ChangesPending { get; set; }

    private int _processId;
    public int ProcessId
    {
        get => _processId;
        private set
        {
            _processId = value;
            ChangesPending = true;
        }
    }

    public string Name { get; set; }
    
    private Func<object[], Task>? _task;

    [NotMapped]
    public Func<object[], Task>? Task
    {
        get => _task;
        set
        {
            _task = value;
            Name = value?.GetMethodInfo().Name ?? string.Empty;
        }
    }
    
    [NotMapped]
    public IStep? NextStep { get; set; }

    private TaskStatus _status = TaskStatus.Created;
    public TaskStatus Status
    {
        get => _status;
        private set
        {
            _status = value;
            ChangesPending = true;
        }
    }
    
    public Step(IRepository<Step> repository)
    {
        _repository = repository;
    }

    public Step() { }

    public void Initialize(Func<object[], Task> task, IStep? nextStep = default)
    {
        Task = task;
        NextStep = nextStep;
        Status = TaskStatus.WaitingToRun;
        ChangesPending = true;
    }

    public async Task SaveChangesAsync()
    {
        await _repository.UpdateAsync(this);
    }

    private async Task UpdateStatusAsync(TaskStatus status)
    {
        Status = status;
        await SaveChangesAsync();
    }
    
    public async Task Run(object? parameter)
    {
        try
        {
            await UpdateStatusAsync(TaskStatus.Running);
            await Task.Invoke(parameter is null ? Array.Empty<object>() : (object[])parameter);
            await UpdateStatusAsync(TaskStatus.RanToCompletion);
        }
        catch
        {
            await UpdateStatusAsync(TaskStatus.Faulted);
            throw;
        }
    }

    public object Key => Id;
}
