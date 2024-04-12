using SchedulerApi.CustomEventArgs;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Organization;
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

    private Desk _desk;

    public Desk Desk
    {
        get => _desk;
        private set
        {
            _desk = value;
            DeskId = value.Id;
        }
    }
    public string DeskId { get; private set; }
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

    private async Task Activate(Desk desk, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var parameters = new object[] { desk, startDateTime, endDateTime, shiftDuration };
        await Proceed(parameters);
    }

    public async Task Run(Desk desk, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        Desk = desk;
        ScheduleStart = startDateTime;
        ScheduleEnd = endDateTime;
        ScheduleShiftDuration = shiftDuration;
        
        await Activate(desk, startDateTime, endDateTime, shiftDuration);

        while (CurrentStep is not null)
        {
            await Proceed();
        }
    }
}
