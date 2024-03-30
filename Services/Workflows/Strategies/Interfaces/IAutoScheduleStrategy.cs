using SchedulerApi.CustomEventArgs;

namespace SchedulerApi.Services.Workflows.Strategies.Interfaces;

public interface IAutoScheduleStrategy : IStrategy
{
        public delegate void TimelineCapturedEventHandler(object source, TimelineCapturedEventArgs e);
        public event TimelineCapturedEventHandler? TimelineCaptured;
}