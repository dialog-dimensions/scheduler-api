using Hangfire;
using Microsoft.AspNetCore.Identity;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.ImageGenerationServices.ScheduleToImageStorage;
using SchedulerApi.Services.ScheduleEngine.Interfaces;
using SchedulerApi.Services.WhatsAppClient.Twilio;
using SchedulerApi.Services.Workflows.Jobs.Classes;
using SchedulerApi.Services.Workflows.Processes.Interfaces;

namespace SchedulerApi.Services.Workflows.Procedures;

public class GptScheduleProcessProcedures : IGptScheduleProcessProcedures
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfigurationSection _params;

    
    private IConfigurationSection JobParams => _params.GetSection("Workflows:AutoScheduleJob");

    public GptScheduleProcessProcedures(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _params = configuration.GetSection("Params");
    }

    public async Task CheckAndInitiateProcessAsync(string deskId)
    {
        // Get Schedule Repository Instance from the Service Provider
        var scheduleRepository = _serviceProvider.GetRequiredService<IScheduleRepository>();
        
        // Get Latest Schedule from Repository
        var latestSchedule = await scheduleRepository.ReadLatestAsync(deskId);
        
        // Don't Start A Process Without Continuation
        if (latestSchedule is null)
        {
            return;
        }
        
        // Don't Start A Process Retroactively
        if (latestSchedule.EndDateTime < DateTime.Now)
        {
            return;
        }
        
        // Check Condition
        var catchRangeHrs = JobParams.GetValue<double>("CatchRangeHrs");
        var timeFromEndHrs = latestSchedule.EndDateTime.Subtract(DateTime.Now).TotalHours;
        var conditionMet = timeFromEndHrs <= catchRangeHrs;
        if (!conditionMet)
        {
            return;
        }
        
        // Condition Met, Initialize Process
        var newStartDateTime = latestSchedule.EndDateTime;
        var newEndDateTime = newStartDateTime.AddDays(_params.GetValue<double>("Schedule:Dur:Default"));
        var newShiftDuration = _params.GetValue<int>("Shift:Dur:Default");
        var newDeskId = latestSchedule.DeskId;
        
        var process = _serviceProvider.GetRequiredService<IGptScheduleProcess>();
        await process.Run(newDeskId, newStartDateTime, newEndDateTime, newShiftDuration);
    }

    public async Task AfterGatheringAsync(int processId)
    {
         // Initialization - Create Service Instances Required for the Procedure
        var scheduleRepository = _serviceProvider.GetRequiredService<IScheduleRepository>();
        var unitRepository = _serviceProvider.GetRequiredService<IUnitRepository>();
        var processRepository = _serviceProvider.GetRequiredService<IAutoScheduleProcessRepository>();
        var userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var scheduler = _serviceProvider.GetRequiredService<IScheduler>();
        var twilio = _serviceProvider.GetRequiredService<ITwilioServices>();
        var backgroundJobClient = _serviceProvider.GetRequiredService<IBackgroundJobClient>();
        var messageCooldown = TimeSpan.FromSeconds(5);

        // Fetch the Schedule and Process from the Database
        var process = await processRepository.ReadAsync(processId);
        if (process is null)
        {
            return;
        }

        var desk = process.Desk;
        var scheduleStartDateTime = process.ScheduleStart;
        
        var schedule = await scheduleRepository.ReadAsync((desk.Id, scheduleStartDateTime));
        if (schedule is null)
        {
            return;
        }
            
        var unitId = schedule.Desk.UnitId;

        // Run Scheduler
        var schedulerResults = await scheduler.RunAsync(schedule);
        await scheduleRepository.AssignEmployees(desk.Id, scheduleStartDateTime, schedulerResults.CompleteSchedule);

        // Publish Initial Proposition to Managers
        var managers = await unitRepository.GetUnitManagers(unitId);
        
        foreach (var manager in managers)
        {
            var id = manager.Id;
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user is null)
            {
                continue;
            }

            var phoneNumber = user.PhoneNumber!;
            var managerName = manager.Name;
            var approveWindowEndDateTime = process.PublishDateTime;
            await twilio.TriggerNotifyManagerFlow(phoneNumber, desk, managerName, scheduleStartDateTime,
                approveWindowEndDateTime);
            await Task.Delay(messageCooldown);
        }
        
        // Create a Delayed Job for Commiting and Publishing the Schedule (This is the Approval Window)
        backgroundJobClient.Schedule<GptProcessAfterApprovalJob>(
            job => job.Execute(processId),
            process.PublishDateTime.Subtract(DateTime.Now)
        );
    }

    public async Task AfterApprovalAsync(int processId)
    {
        // Initialization - Get Required Services
        var scheduleRepository = _serviceProvider.GetRequiredService<IScheduleRepository>();
        var employeeRepository = _serviceProvider.GetRequiredService<IEmployeeRepository>();
        var processRepository = _serviceProvider.GetRequiredService<IAutoScheduleProcessRepository>();
        var scheduleImagePublisher = _serviceProvider.GetRequiredService<IScheduleImagePublisher>();
        var userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var twilio = _serviceProvider.GetRequiredService<ITwilioServices>();
        var messageCooldown = TimeSpan.FromSeconds(5);
        
        // Commit Schedule
        var process = await processRepository.ReadAsync(processId);
        if (process is null)
        {
            return;
        }

        var deskId = process.DeskId;
        var scheduleStartDateTime = process.ScheduleStart;
        
        var schedule = await scheduleRepository.ReadAsync((deskId, scheduleStartDateTime));
        if (schedule is null)
        {
            return;
        }

        var report = await scheduleRepository.GetScheduleReport(schedule);
        foreach (var increments in report.Increments)
        {
            await employeeRepository.IncrementRegularBalance(increments.EmployeeId, increments.RegularIncrement);
            await employeeRepository.IncrementDifficultBalance(increments.EmployeeId, increments.DifficultIncrement);
        }
        
        // Create Personalized Schedule Images for the Employees
        var data = await scheduleRepository.GetScheduleData(deskId, scheduleStartDateTime);
        data.Schedule = schedule;

        foreach (var employee in data.Employees)
        {
            await scheduleImagePublisher.PublishScheduleImage(schedule, employee);
        }
        
        // Publish Schedule
        foreach (var employee in data.Employees)
        {
            var user = await userManager.FindByIdAsync(employee.Id.ToString());
            if (user is null)
            {
                continue;
            }

            var phoneNumber = user.PhoneNumber!;
            var userName = employee.Name;

            await twilio.TriggerPublishShiftsMediaFlow(phoneNumber, userName, schedule, employee);
            await Task.Delay(messageCooldown);
        }
    }
}