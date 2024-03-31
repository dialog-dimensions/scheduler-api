using Microsoft.AspNetCore.Identity;
using SchedulerApi.CustomEventArgs;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities.Factories;
using SchedulerApi.Services.ScheduleEngine.Interfaces;
using SchedulerApi.Services.WhatsAppClient.Twilio;
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

    public DateTime ProcessStart { get; private set; }
    public DateTime FileWindowEnd { get; private set; }
    public DateTime ProcessEnd { get; private set; }
    public DateTime ScheduleStart { get; private set; }


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
        IServiceProvider serviceProvider
    ) : base(serviceProvider)
    {
        _scheduleRepository = scheduleRepository;
        _employeeRepository = employeeRepository;
        _scheduleFactory = scheduleFactory;
        _scheduler = scheduler;
        _userManager = userManager;
        _twilio = twilio;
        
        _params = configuration.GetSection("Params:Processes:AutoSchedule");
        
        Construct();
    }

    private async Task CreateShifts(DateTime start, DateTime end, int shiftDuration)
    {
        CaptureProcessTimeline(start);
        await _scheduleRepository.CreateAsync(
            _scheduleFactory.FromParameters(start, end, shiftDuration)
            );
    }

    private void CaptureProcessTimeline(DateTime start)
    {
        ScheduleStart = start;
        ProcessStart = DateTime.Now;
        FileWindowEnd = DateTime.Now.AddHours(_params.GetValue<double>("FileWindowDurHrs"));
        ProcessEnd = start.AddHours(-_params.GetValue<double>("HeadsUpDurHrs"));
        
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
        var start = (DateTime)parameters[0];
        var end = (DateTime)parameters[1];
        var shiftDuration = (int)parameters[2];

        await CreateShifts(start, end, shiftDuration);
    }

    private async Task NotifyGather()
    {
        var activeEmployees = await _employeeRepository.ReadAllActiveAsync();
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
            
            await _twilio.TriggerCallToFileFlow(phoneNumber, userName, ScheduleStart, FileWindowEnd);
            await Task.Delay(_messageBufferTime);
        }
    }

    private async Task NotifyGatherAsync(object[] parameters)
    {
        await NotifyGather();
    }

    private static async Task Await(TimeSpan timeSpan)
    {
        var delay = timeSpan < TimeSpan.Zero ? TimeSpan.Zero : timeSpan;
        await Task.Delay(delay);
    }

    private async Task AwaitGather()
    {
        await Await(FileWindowEnd.Subtract(DateTime.Now));
    }

    private async Task AwaitGatherAsync(object[] parameters)
    {
        await AwaitGather();
    }

    private async Task RunScheduler()
    {
        var schedule = await _scheduleRepository.ReadAsync(ScheduleStart);
        if (schedule is null)
        {
            throw new KeyNotFoundException("Schedule not found in database.");
        }

        var results = await _scheduler.RunAsync(schedule);
        await _scheduleRepository.AssignEmployees(ScheduleStart, results.CompleteSchedule);
    }

    private async Task RunSchedulerAsync(object[] parameters)
    {
        await RunScheduler();
    }

    private async Task NotifyManagerScheduleReady()
    {
        var manager = (await _employeeRepository.ReadAllAsync()).FirstOrDefault(emp => emp.Role == "Manager");
        if (manager is null)
        {
            return;
        }

        var user = await _userManager.FindByIdAsync(manager.Id.ToString());
        if (user is null)
        {
            return;
        }

        Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Notifying Manager {manager.Name} on {user.PhoneNumber} Schedule is Ready for Review.");
        await _twilio.TriggerNotifyManagerFlow(user.PhoneNumber!, manager.Name, ScheduleStart, ProcessEnd);
    }

    private async Task NotifyManagerScheduleReadyAsync(object[] parameters)
    {
        await NotifyManagerScheduleReady();
    }

    private async Task AwaitApproval()
    {
        var actualApproveWindowDuration = ProcessEnd.Subtract(DateTime.Now);
        await Await(actualApproveWindowDuration);
    }

    private async Task AwaitApprovalAsync(object[] parameters)
    {
        await AwaitApproval();
    }

    private async Task CommitSchedule()
    {
        var schedule = await _scheduleRepository.ReadAsync(ScheduleStart);
        if (schedule is null)
        {
            return;
        }

        var report = await _scheduleRepository.GetScheduleReport(schedule);
        foreach (var increments in report.Increments)
        {
            await _employeeRepository.IncrementRegularBalance(increments.EmployeeId, increments.RegularIncrement);
            await _employeeRepository.IncrementDifficultBalance(increments.EmployeeId, increments.DifficultIncrement);
        }
    }

    private async Task CommitScheduleAsync(object[] parameters)
    {
        await CommitSchedule();
    }

    private async Task PublishSchedule()
    {
        var data = await _scheduleRepository.GetScheduleData(ScheduleStart);
        var schedule = await _scheduleRepository.ReadAsync(ScheduleStart);
        if (schedule is null)
        {
            return;
        }
        data.Schedule = schedule;
        
        foreach (var employee in data.Employees)
        {
            var user = await _userManager.FindByIdAsync(employee.Id.ToString());
            await _twilio.TriggerPublishShiftsFlow(user!.PhoneNumber!, employee.Name, data.Schedule.StartDateTime, 
                data.Schedule.EndDateTime);
            
            await Task.Delay(_messageBufferTime);
        }
    }

    private async Task PublishScheduleAsync(object[] parameters)
    {
        await PublishSchedule();
    }


    private void Construct()
    {
        foreach (var task in new[]
                 {
                     CreateShiftsAsync,
                     NotifyGatherAsync,
                     AwaitGatherAsync,
                     RunSchedulerAsync,
                     NotifyManagerScheduleReadyAsync,
                     AwaitApprovalAsync,
                     CommitScheduleAsync,
                     PublishScheduleAsync
                 })
        {
            AddStep(task);
        }
    }
}
