using SchedulerApi.CustomEventArgs;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;
using SchedulerApi.Services.Workflows.Strategies.Interfaces;

namespace SchedulerApi.Services.Workflows.Processes.Classes;

public class AutoScheduleProcess : Process, IAutoScheduleProcess
{
    private readonly IAutoScheduleProcessRepository _autoRepository;
    private readonly IServiceProvider _serviceProvider;
    
    // TIMELINE
    public DateTime ProcessStart { get; private set; }
    public DateTime FileWindowEnd { get; private set; }
    public DateTime PublishDateTime { get; private set; }
    
    
    // SCHEDULE ATTRIBUTES
    public DateTime ScheduleStart { get; private set; }
    public DateTime ScheduleEnd { get; private set; }
    public int ScheduleShiftDuration { get; private set; }


    public AutoScheduleProcess(IServiceProvider serviceProvider, IAutoScheduleProcessRepository autoRepository)
    {
        _autoRepository = autoRepository;
        _serviceProvider = serviceProvider;
        Initialize();
    }

    public AutoScheduleProcess() { }


    protected override async Task SaveChangesAsync()
    {
        await _autoRepository.UpdateAsync(this);
        SaveChangesPending = false;
    }

    private void HandleTimelineCaptured(object source, TimelineCapturedEventArgs e)
    {
        ProcessStart = e.ProcessStart;
        FileWindowEnd = e.FileWindowEnd;
        PublishDateTime = e.ProcessEnd;
        SaveChangesPending = true;
    }

    private void Initialize()
    {
        var strategy = _serviceProvider.GetRequiredService<IAutoScheduleStrategy>();
        strategy.TimelineCaptured += HandleTimelineCaptured;
        base.Initialize(strategy);
    }

    private async Task Activate(DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var parameters = new object[] { startDateTime, endDateTime, shiftDuration };
        await Proceed(parameters);
    }

    public async Task Run(DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        ScheduleStart = startDateTime;
        ScheduleEnd = endDateTime;
        ScheduleShiftDuration = shiftDuration;
        
        await Activate(startDateTime, endDateTime, shiftDuration);

        while (CurrentStep is not null)
        {
            await Proceed();
        }
    }
}
