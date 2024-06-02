using SchedulerApi.Services.Workflows.Jobs.Interfaces;
using SchedulerApi.Services.Workflows.Procedures;

namespace SchedulerApi.Services.Workflows.Jobs.Classes;

public class GptProcessInitiationJob : IGptProcessInitiationJob
{
    private readonly IServiceProvider _serviceProvider;

    public GptProcessInitiationJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Execute(string deskId)
    {
        using var scope = _serviceProvider.CreateScope();
        var procedures = scope.ServiceProvider.GetRequiredService<IGptScheduleProcessProcedures>();
        await procedures.CheckAndInitiateProcessAsync(deskId);
    }
}
