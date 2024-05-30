using System.ComponentModel.DataAnnotations.Schema;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;
using SchedulerApi.Services.Workflows.Steps;
using SchedulerApi.Services.Workflows.Strategies;

namespace SchedulerApi.Services.Workflows.Processes;

public class Process : IProcess
{
    protected virtual IRepository<Process> Repository { get; set; }

    private int _id;
    public int Id
    {
        get => _id;
        set
        {
            _id = value;
            var s = Strategy?.InitialStep;
            while (s is not null)
            {
                s.Process = this;
                s = s.NextStep;
            }
        }
    }
    public IStrategy? Strategy { get; private set; }
    
    protected bool SaveChangesPending { get; set; }

    private TaskStatus _status = TaskStatus.Created;

    public TaskStatus Status
    {
        get => _status;
        private set
        {
            _status = value;
            SaveChangesPending = true;
        }
    }

    private IStep? _currentStep;

    [NotMapped]
    public IStep? CurrentStep
    {
        get => _currentStep;
        private set
        {
            _currentStep = value;
            CurrentStepName = value?.Task.Method.Name ?? string.Empty;
            
            if (value is null)
            {
                Status = TaskStatus.RanToCompletion;
            }
            else
            {
                value.Process = this;
            }

            SaveChangesPending = true;
        }
    }

    [Column("CurrentStep")]
    public string CurrentStepName { get; set; }
    
    
    protected Process()
    {

    }

    protected void Initialize(IStrategy strategy)
    {
        if (!strategy.HasSteps)
        {
            throw new ArgumentException("Empty strategy given to the process.");
        }
        
        Strategy = strategy;
        CurrentStep = Strategy.InitialStep!;
        Status = TaskStatus.WaitingToRun;
        SaveChangesPending = true;
    }

    protected virtual async Task SaveChangesAsync()
    {
        if (!SaveChangesPending) return;
        await Repository.UpdateAsync(this);
        SaveChangesPending = false;
    }
    
    
    public async Task Proceed(object? parameter = default)
    {
        if (SaveChangesPending)
        {
            await SaveChangesAsync();
        }
        
        if (Status == TaskStatus.WaitingToRun)
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
        }
        finally
        {
            await SaveChangesAsync();
        }
    }

    [NotMapped]
    public object Key => Id;
}
