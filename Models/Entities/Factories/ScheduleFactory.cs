using SchedulerApi.Models.Organization;

namespace SchedulerApi.Models.Entities.Factories;

public class ScheduleFactory : IScheduleFactory
{
    public Schedule? FromShifts(IEnumerable<Shift> shifts)
    {
        var orderedShifts = shifts.OrderBy(shift => shift.StartDateTime).ToList();
        if (orderedShifts.Count == 0) return null;

        var result = new Schedule();
        result.AddRange(orderedShifts);
        return result;
    }

    public Schedule FromParameters(Desk desk, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var result = new Schedule();
        
        var shiftStartDateTime = startDateTime;
        var shiftEndDateTime = startDateTime.AddHours(shiftDuration);
        while (shiftStartDateTime < endDateTime)
        {
            result.Add(new Shift
            {
                ScheduleStartDateTime = startDateTime, 
                Desk = desk,
                StartDateTime = shiftStartDateTime, 
                EndDateTime = shiftEndDateTime
            });
            shiftStartDateTime = shiftEndDateTime;
            shiftEndDateTime = shiftEndDateTime.AddHours(shiftDuration);
        }

        return result;
    }

    public Schedule Copy(Schedule schedule)
    {
        var result = new Schedule();
        result.AddRange(schedule.Select(shift => shift.Copy()));
        return result;
    }
}
