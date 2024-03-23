using SchedulerApi.Services.Workflows.Processes.Factories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;

namespace SchedulerApi.Services.Workflows.Processes.Factories.Classes;

public class AutoScheduleProcessFactory : IAutoScheduleProcessFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public AutoScheduleProcessFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IAutoScheduleProcess Create()
    {
        return _serviceProvider.GetRequiredService<IAutoScheduleProcess>();
    }
}
