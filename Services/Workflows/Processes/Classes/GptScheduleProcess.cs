using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;
using SchedulerApi.Services.Workflows.Strategies.Interfaces;

namespace SchedulerApi.Services.Workflows.Processes.Classes;

public class GptScheduleProcess : AutoScheduleProcess, IGptScheduleProcess
{
    public GptScheduleProcess(IAutoScheduleProcessRepository processRepository, IDeskRepository deskRepository, IServiceProvider serviceProvider)
    : base(serviceProvider, processRepository, deskRepository, false)
    {
        Initialize();
    }
    
    public new void Initialize()
    {
        var strategy = ServiceProvider.GetRequiredService<IGptStrategy>();
        strategy.TimelineCaptured += HandleTimelineCaptured;
        base.Initialize(strategy);
    }
}
