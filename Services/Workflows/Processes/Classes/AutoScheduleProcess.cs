using SchedulerApi.CustomEventArgs;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.Workflows.Processes.Interfaces;
using SchedulerApi.Services.Workflows.Strategies.Interfaces;

namespace SchedulerApi.Services.Workflows.Processes.Classes;

public class AutoScheduleProcess : Process, IAutoScheduleProcess
{
    protected IAutoScheduleProcessRepository AutoRepository { get; set; }
    protected IDeskRepository DeskRepository { get; set; }
    protected IServiceProvider ServiceProvider { get; set; }

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


    public AutoScheduleProcess(
        IServiceProvider serviceProvider, 
        IAutoScheduleProcessRepository autoRepository, 
        IDeskRepository deskRepository, 
        IAutoScheduleStrategy strategy,
        bool initialize = true)
    {
        AutoRepository = autoRepository;
        DeskRepository = deskRepository;
        ServiceProvider = serviceProvider;
        
        if (initialize)
        {
            Initialize(strategy);
        }
    }

    public AutoScheduleProcess() { }


    protected override async Task SaveChangesAsync()
    {
        await AutoRepository.UpdateAsync(this);
        SaveChangesPending = false;
    }

    protected void HandleTimelineCaptured(object source, TimelineCapturedEventArgs e)
    {
        ProcessStart = e.ProcessStart;
        FileWindowEnd = e.FileWindowEnd;
        PublishDateTime = e.ProcessEnd;
        SaveChangesPending = true;
    }

    protected sealed override void Initialize(IStrategy strategy)
    {
        if (strategy is IAutoScheduleStrategy autoScheduleStrategy)
        {
            autoScheduleStrategy.ProcessId = Id;
            autoScheduleStrategy.TimelineCaptured += HandleTimelineCaptured;
        }
        
        base.Initialize(strategy);
    }

    protected async Task Activate(Desk desk, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var parameters = new object[] { desk, startDateTime, endDateTime, shiftDuration };
        await Proceed(parameters);
    }

    public async Task<int> Run(string deskId, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var desk = await DeskRepository.ReadAsync(deskId);
        if (desk is null)
        {
            return 0;
        }
        
        Desk = desk;
        ScheduleStart = startDateTime;
        ScheduleEnd = endDateTime;
        ScheduleShiftDuration = shiftDuration;

        var key = await AutoRepository.CreateAsync(this);
        Strategy!.ProcessId = Id;
        
        await Activate(desk, startDateTime, endDateTime, shiftDuration);

        while (CurrentStep is not null)
        {
            await Proceed();
        }

        if (key is not int id)
        {
            return 0;
        }
        
        return id;
    }
}
