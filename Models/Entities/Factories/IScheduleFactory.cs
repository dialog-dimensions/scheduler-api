namespace SchedulerApi.Models.Entities.Factories;

public interface IScheduleFactory
{
    Schedule? FromShifts(IEnumerable<Shift> shifts);
    Schedule FromParameters(DateTime startDateTime, DateTime endDateTime, int shiftDuration);
    Schedule Copy(Schedule schedule);
}