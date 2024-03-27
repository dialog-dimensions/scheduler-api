﻿using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Factories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;

namespace SchedulerApi.Services.Workflows.Scanners;

public class AutoScheduleScanner : IAutoScheduleScanner
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    private IAutoScheduleProcess? _processInProgress;
    public IAutoScheduleProcess? ProcessInProgress
    {
        get
        {
            if (_processInProgress is { Status: TaskStatus.Created or TaskStatus.Running })
            {
                return _processInProgress;
            }
    
            ProcessInProgress = null;
            return null;
        }
        private set => _processInProgress = value;
    }

    private int DefaultScheduleDuration { get; }
    private int DefaultShiftDuration { get; }

    public TimeSpan CatchRangeDuration { get; set; }
    public TimeSpan CycleDuration { get; set; }
    

    public AutoScheduleScanner(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        
        var paramsSection = configuration.GetSection("Params");
        var myParamsSection = paramsSection.GetSection("Workflows:AutoScheduleScanner");
        
        DefaultScheduleDuration = paramsSection.GetValue<int>("Schedule:Dur:Default");
        DefaultShiftDuration = paramsSection.GetValue<int>("Shift:Dur:Default");

        CatchRangeDuration = TimeSpan.FromHours(myParamsSection.GetValue<double>("CatchRangeHrs"));
        CycleDuration = TimeSpan.FromMinutes(myParamsSection.GetValue<double>("CycleDurMts"));
    }
    
    public bool ShouldRun { get; set; }
    
    public async Task Run()
    {
        if (!ShouldRun)
        {
            return;
        }
        
        // Scan indefinitely.
        while(ShouldRun)
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            
            Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Entered Scan Iteration");
            
            var latestSchedule =  await scope.ServiceProvider.GetRequiredService<IScheduleRepository>().ReadLatestAsync();
            if (latestSchedule is null)
            {
                Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Empty Latest Schedule, Breaking.");
                
                ShouldRun = false;
                break;
            }

            if (latestSchedule.EndDateTime.Subtract(DateTime.Now) <= CatchRangeDuration)
            {
                Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Condition Met.");
                
                var newStartDateTime = latestSchedule.EndDateTime;
                var newEndDateTime = latestSchedule.EndDateTime.AddDays(DefaultScheduleDuration);
                
                Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Crating and Starting Process...");
                var newProcess = scope.ServiceProvider.GetRequiredService<IAutoScheduleProcess>();
                ProcessInProgress = newProcess;
                await newProcess.Run(newStartDateTime, newEndDateTime, DefaultShiftDuration);
            }

            Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Loop Delay until {DateTime.Now.Add(CycleDuration)}.");
            await Task.Delay(CycleDuration);
        }
    }

    public async Task Wake()
    {
        ShouldRun = true;
        await Run();
    }

    public void Terminate()
    {
        ShouldRun = false;
    }
}