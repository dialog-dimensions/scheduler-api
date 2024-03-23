using SchedulerApi.CustomEventArgs;
using SchedulerApi.Services.Workflows.Processes.Interfaces;
using SchedulerApi.Services.Workflows.Strategies.Classes;
using SchedulerApi.Services.Workflows.Strategies.Interfaces;

namespace SchedulerApi.Services.Workflows.Processes.Classes;

public class AutoScheduleProcess : Process, IAutoScheduleProcess
{
    // TIMELINE
    public DateTime ProcessStart { get; private set; }
    public DateTime FileWindowEnd { get; private set; }
    public DateTime PublishDateTime { get; private set; }
    
    public AutoScheduleProcess(IServiceProvider serviceProvider) :
        base(serviceProvider.GetRequiredService<IAutoScheduleStrategy>())
    {
        ((AutoScheduleStrategy)Strategy).TimelineCaptured += HandleTimelineCaptured;
    }

    private void HandleTimelineCaptured(object source, TimelineCapturedEventArgs e)
    {
        ProcessStart = e.ProcessStart;
        FileWindowEnd = e.FileWindowEnd;
        PublishDateTime = e.ProcessEnd;
    }

    public async Task Initialize(DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var parameters = new object[] { startDateTime, endDateTime, shiftDuration };
        await Proceed(parameters);
    }

    public async Task Run(DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        await Initialize(startDateTime, endDateTime, shiftDuration);

        while (CurrentStep is not null)
        {
            await Proceed();
        }
    }
}
