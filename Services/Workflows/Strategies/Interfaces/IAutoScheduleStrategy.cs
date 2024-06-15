using SchedulerApi.CustomEventArgs;

namespace SchedulerApi.Services.Workflows.Strategies.Interfaces;

public interface IAutoScheduleStrategy : IStrategy
{
        public delegate void TimelineCapturedEventHandler(object source, TimelineCapturedEventArgs e);
        public event TimelineCapturedEventHandler? TimelineCaptured;
        
        public delegate void NextPhaseJobIdCapturedEventHandler(object source, NextPhaseJobIdCapturedEventArgs e);
        public event NextPhaseJobIdCapturedEventHandler? JobIdCaptured;
}