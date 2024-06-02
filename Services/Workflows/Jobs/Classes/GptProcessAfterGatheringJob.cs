using SchedulerApi.Services.Workflows.Jobs.Interfaces;
using SchedulerApi.Services.Workflows.Procedures;

namespace SchedulerApi.Services.Workflows.Jobs.Classes;

public class GptProcessAfterGatheringJob : IGptProcessAfterGatheringJob
{
    private readonly IServiceProvider _serviceProvider;

    public GptProcessAfterGatheringJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Execute(int processId)
    {
        using var scope = _serviceProvider.CreateScope();
        var procedures = scope.ServiceProvider.GetRequiredService<IGptScheduleProcessProcedures>();
        await procedures.AfterGatheringAsync(processId);
    }
}