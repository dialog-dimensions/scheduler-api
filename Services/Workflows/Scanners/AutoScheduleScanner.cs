using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;

namespace SchedulerApi.Services.Workflows.Scanners;

public class AutoScheduleScanner : IAutoScheduleScanner
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAutoScheduleProcess _process;
    
    private int DefaultScheduleDuration { get; }
    private int DefaultShiftDuration { get; }

    public TimeSpan CatchRangeDuration { get; set; }
    public TimeSpan CycleDuration { get; set; }
    

    public AutoScheduleScanner(IScheduleRepository scheduleRepository, 
        IAutoScheduleProcess autoScheduleProcess, IConfiguration configuration)
    {
        _scheduleRepository = scheduleRepository;
        _process = autoScheduleProcess;
        
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
            Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Entered Scan Iteration");
            
            var latestSchedule = await _scheduleRepository.ReadLatestAsync();
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

                Console.WriteLine($"{DateTime.Now:MM-dd HH:mm:ss} Starting Process...");
                await _process.Run(newStartDateTime, newEndDateTime, DefaultShiftDuration);
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
