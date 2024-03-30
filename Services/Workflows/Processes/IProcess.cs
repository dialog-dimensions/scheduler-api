using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchedulerApi.Models.Interfaces;
using SchedulerApi.Services.Workflows.Steps;
using SchedulerApi.Services.Workflows.Strategies;

namespace SchedulerApi.Services.Workflows.Processes;

public interface IProcess : IKeyProvider
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    IStrategy? Strategy { get; }
    TaskStatus Status { get; }
    IStep? CurrentStep { get; }
    Task Proceed(object? parameter);
}
