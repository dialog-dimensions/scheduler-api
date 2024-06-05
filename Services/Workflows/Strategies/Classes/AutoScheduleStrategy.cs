using Hangfire;
using Microsoft.AspNetCore.Identity;
using SchedulerApi.CustomEventArgs;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities.Factories;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ScheduleEngine.Interfaces;
using SchedulerApi.Services.WhatsAppClient.Twilio;
using SchedulerApi.Services.Workflows.Jobs.Classes;
using SchedulerApi.Services.Workflows.Strategies.Interfaces;

namespace SchedulerApi.Services.Workflows.Strategies.Classes;

public sealed class AutoScheduleStrategy : Strategy, IAutoScheduleStrategy
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IScheduleFactory _scheduleFactory;
    private readonly IScheduler _scheduler;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITwilioServices _twilio;
    private readonly IConfigurationSection _params;
    private readonly TimeSpan _messageBufferTime = TimeSpan.FromSeconds(5);
    private readonly IBackgroundJobClient _backgroundJobClient;

    public DateTime ProcessStart { get; private set; }
    public DateTime FileWindowEnd { get; private set; }
    public DateTime ProcessEnd { get; private set; }
    public DateTime ScheduleStart { get; private set; }
    
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

    public delegate void TimelineCapturedEventHandler(object source, TimelineCapturedEventArgs e);
    public event IAutoScheduleStrategy.TimelineCapturedEventHandler? TimelineCaptured;
    
    public AutoScheduleStrategy(
        IScheduleRepository scheduleRepository,
        IEmployeeRepository employeeRepository,
        IScheduleFactory scheduleFactory,
        IScheduler scheduler,
        UserManager<IdentityUser> userManager,
        ITwilioServices twilio,
        IConfiguration configuration,
        IServiceProvider serviceProvider, 
        IBackgroundJobClient backgroundJobClient) : base(serviceProvider)
    {
        _scheduleRepository = scheduleRepository;
        _employeeRepository = employeeRepository;
        _scheduleFactory = scheduleFactory;
        _scheduler = scheduler;
        _userManager = userManager;
        _twilio = twilio;
        _backgroundJobClient = backgroundJobClient;

        _params = configuration.GetSection("Params:Processes:AutoSchedule");
        
        Construct();
    }

    private void BeforeCreatingShifts(Desk desk, DateTime start)
    {
        Desk = desk;
        CaptureProcessTimeline(start);
    }
    
    private async Task CreateShifts(Desk desk, DateTime start, DateTime end, int shiftDuration)
    {
        BeforeCreatingShifts(desk, start);
        await _scheduleRepository.CreateAsync(
            _scheduleFactory.FromParameters(desk, start, end, shiftDuration)
            );
    }

    private void CaptureProcessTimeline(DateTime start)
    {
        var fileWindowDuration = Desk.ProcessParameters.FileWindowDuration;
        var headsUpDuration = Desk.ProcessParameters.HeadsUpDuration;
        
        ProcessStart = DateTime.Now;
        FileWindowEnd = DateTime.Now.Add(fileWindowDuration);
        ProcessEnd = start.Subtract(headsUpDuration);
        ScheduleStart = start;
        
        OnTimelineCaptured();
    }

    private void OnTimelineCaptured()
    {
        TimelineCaptured?.Invoke(this, new TimelineCapturedEventArgs
        {
            ProcessStart = ProcessStart, 
            FileWindowEnd = FileWindowEnd, 
            ProcessEnd = ProcessEnd
        });
    }

    private async Task CreateShiftsAsync(IReadOnlyList<object> parameters)
    {
        var desk = (Desk)parameters[0];
        var start = (DateTime)parameters[1];
        var end = (DateTime)parameters[2];
        var shiftDuration = (int)parameters[3];

        await CreateShifts(desk, start, end, shiftDuration);
    }

    private async Task NotifyGather()
    {
        var activeEmployees = await _employeeRepository.ReadAllActiveAsync(_desk.Id);
        foreach (var employee in activeEmployees)
        {
            var id = employee.Id.ToString();
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                continue;
            }

            var userName = employee.Name;
            var phoneNumber = user.PhoneNumber!;
            
            await _twilio.TriggerCallToFileFlow(phoneNumber, Desk, userName, ScheduleStart, FileWindowEnd);
            await Task.Delay(_messageBufferTime);
        }
    }

    private async Task NotifyGatherAsync(object[] parameters)
    {
        await NotifyGather();
    }

    // private static async Task Await(TimeSpan timeSpan)
    // {
    //     var delay = timeSpan < TimeSpan.Zero ? TimeSpan.Zero : timeSpan;
    //     await Task.Delay(delay);
    // }

    // private async Task AwaitGather()
    // {
    //     await Await(FileWindowEnd.Subtract(DateTime.Now));
    // }
    
    // private async Task AwaitGatherAsync(object[] parameters)
    // {
    //     await AwaitGather();
    // }
    
    private void ScheduleNextPhase()
    {
        _backgroundJobClient.Schedule<GptProcessAfterGatheringJob>(
            job => job.Execute(ProcessId),
            FileWindowEnd.Subtract(DateTime.Now)
        );
    }

    private async Task ScheduleNextPhaseAsync(object[] parameters)
    {
        await Task.Delay(10);
        ScheduleNextPhase();
    }

    private async Task RunScheduler()
    {
        var schedule = await _scheduleRepository.ReadAsync((Desk.Id, ScheduleStart));
        if (schedule is null)
        {
            throw new KeyNotFoundException("Schedule not found in database.");
        }

        var results = await _scheduler.RunAsync(schedule);
        await _scheduleRepository.AssignEmployees(Desk.Id, ScheduleStart, results.CompleteSchedule);
    }

    // private async Task RunSchedulerAsync(object[] parameters)
    // {
    //     await RunScheduler();
    // }

    // private async Task NotifyManagerScheduleReady()
    // {
    //     var managers = (await _employeeRepository.GetUnitManagers(Desk.UnitId)).ToList();
    //     if (managers.Count == 0)
    //     {
    //         return;
    //     }
    //
    //     var users = new List<IdentityUser>();
    //     foreach (var manager in managers)
    //     {
    //         var user = await _userManager.FindByIdAsync(manager.Id.ToString());
    //         if (user is null)
    //         {
    //             continue;
    //         }
    //
    //         users.Add(user);
    //     }
    //
    //     if (users.Count == 0)
    //     {
    //         return;
    //     }
    //
    //     foreach (var user in users)
    //     {
    //         var manager = managers.Find(manager => manager.Id.ToString() == user.Id);
    //         if (manager is null)
    //         {
    //             throw new KeyNotFoundException("problem linking a manager instance to the manager user instance");
    //         }
    //         
    //         await _twilio.TriggerNotifyManagerFlow(user.PhoneNumber!, Desk, manager.Name, ScheduleStart, ProcessEnd);
    //         await Await(_messageBufferTime);
    //     }
    // }

    // private async Task NotifyManagerScheduleReadyAsync(object[] parameters)
    // {
    //     await NotifyManagerScheduleReady();
    // }

    // private async Task AwaitApproval()
    // {
    //     var actualApproveWindowDuration = ProcessEnd.Subtract(DateTime.Now);
    //     await Await(actualApproveWindowDuration);
    // }

    // private async Task AwaitApprovalAsync(object[] parameters)
    // {
    //     await AwaitApproval();
    // }

    // private async Task CommitSchedule()
    // {
    //     var schedule = await _scheduleRepository.ReadAsync((Desk.Id, ScheduleStart));
    //     if (schedule is null)
    //     {
    //         return;
    //     }
    //
    //     var report = await _scheduleRepository.GetScheduleReport(schedule);
    //     foreach (var increments in report.Increments)
    //     {
    //         await _employeeRepository.IncrementRegularBalance(increments.EmployeeId, increments.RegularIncrement);
    //         await _employeeRepository.IncrementDifficultBalance(increments.EmployeeId, increments.DifficultIncrement);
    //     }
    // }
    
    // private async Task CommitScheduleAsync(object[] parameters)
    // {
    //     await CommitSchedule();
    // }

    // private async Task PublishSchedule()
    // {
    //     var data = await _scheduleRepository.GetScheduleData(DeskId, ScheduleStart);
    //     var schedule = await _scheduleRepository.ReadAsync((DeskId, ScheduleStart));
    //     if (schedule is null)
    //     {
    //         return;
    //     }
    //     data.Schedule = schedule;
    //     
    //     foreach (var employee in data.Employees)
    //     {
    //         var user = await _userManager.FindByIdAsync(employee.Id.ToString());
    //         await _twilio.TriggerPublishShiftsFlow(user!.PhoneNumber!, Desk, employee.Name, data.Schedule.StartDateTime, 
    //             data.Schedule.EndDateTime);
    //         
    //         await Task.Delay(_messageBufferTime);
    //     }
    // }
    
    // private async Task PublishScheduleAsync(object[] parameters)
    // {
    //     await PublishSchedule();
    // }


    private void Construct()
    {
        foreach (var task in new[]
                 {
                     CreateShiftsAsync,
                     NotifyGatherAsync,
                     ScheduleNextPhaseAsync
                     // RunSchedulerAsync,
                     // NotifyManagerScheduleReadyAsync,
                     // AwaitApprovalAsync,
                     // CommitScheduleAsync,
                     // PublishScheduleAsync
                 })
        {
            AddStep(task);
        }
    }
}
