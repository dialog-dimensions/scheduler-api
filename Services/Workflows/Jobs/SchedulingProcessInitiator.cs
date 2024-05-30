using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes;
using SchedulerApi.Services.Workflows.Processes.Interfaces;

namespace SchedulerApi.Services.Workflows.Jobs;

public class SchedulingProcessInitiator : ISchedulingProcessInitiator
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IConfigurationSection _params;
    private readonly IServiceProvider _serviceProvider;

    private IConfigurationSection JobParams => _params.GetSection("Workflows:Job");
    
    public SchedulingProcessInitiator(IScheduleRepository scheduleRepository, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _scheduleRepository = scheduleRepository;
        _serviceProvider = serviceProvider;
        _params = configuration.GetSection("Params");
    }

    public async Task<bool> CheckAndInitiateProcessAsync(string deskId, string strategyName = "gpt")
    {
        // Get Latest Schedule from Repository
        var latestSchedule = await _scheduleRepository.ReadLatestAsync(deskId);
        if (latestSchedule is null)
        {
            return false;
        }
        
        // Check Condition
        var catchRangeHrs = JobParams.GetValue<double>("CatchRangeHrs");
        var timeFromEndHrs = latestSchedule.EndDateTime.Subtract(DateTime.Now).TotalHours;
        var conditionMet = timeFromEndHrs <= catchRangeHrs;
        if (!conditionMet)
        {
            return false;
        }
        
        // Condition Met, Initialize Process
        var newStartDateTime = latestSchedule.EndDateTime;
        var newEndDateTime = newStartDateTime.AddDays(_params.GetValue<double>("Schedule:Dur:Default"));
        var newShiftDuration = _params.GetValue<int>("Shift:Dur:Default");
        var newDeskId = latestSchedule.DeskId;

        IAutoScheduleProcess process;
        
        if (strategyName == "gpt")
        {
            process = _serviceProvider.GetRequiredService<IGptScheduleProcess>();
        }
        else if (strategyName == "web")
        {
            process = _serviceProvider.GetRequiredService<IAutoScheduleProcess>();
        }
        else
        {
            return false;
        }
        
        await process.Run(newDeskId, newStartDateTime, newEndDateTime, newShiftDuration);
        return true;
    }
    
    
}