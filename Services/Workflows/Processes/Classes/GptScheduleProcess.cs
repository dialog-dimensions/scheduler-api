using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;
using SchedulerApi.Services.Workflows.Strategies.Interfaces;

namespace SchedulerApi.Services.Workflows.Processes.Classes;

public class GptScheduleProcess : AutoScheduleProcess, IGptScheduleProcess
{
    public GptScheduleProcess(
        IAutoScheduleProcessRepository processRepository, 
        IDeskRepository deskRepository, 
        IServiceProvider serviceProvider,
        IGptStrategy strategy)
    : base(serviceProvider, processRepository, deskRepository, strategy, false)
    {
        Initialize(strategy);
    }
    
    public void Initialize(IGptStrategy strategy)
    {
        strategy.ProcessId = Id;
        strategy.TimelineCaptured += HandleTimelineCaptured;
        strategy.JobIdCaptured += HandleNextPhaseJobIdCaptured;
        base.Initialize(strategy);
    }
}
