namespace SchedulerApi.Models.Entities.Factories;

public class ScheduleFactory : IScheduleFactory
{
    public Schedule? FromShifts(IEnumerable<Shift> shifts)
    {
        var orderedShifts = shifts.OrderBy(shift => shift.StartDateTime).ToList();
        if (orderedShifts.Count == 0) return null;

        var startDateTime = orderedShifts[0].StartDateTime;
        var endDateTime = orderedShifts[^1].EndDateTime;
        var shiftDuration = (int)orderedShifts[0].EndDateTime.Subtract(startDateTime).TotalHours;

        var result = new Schedule
        {
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            ShiftDuration = shiftDuration
        };
        result.AddRange(orderedShifts);
        
        return result;
    }

    public Schedule FromParameters(DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var result = new Schedule
        {
            StartDateTime = startDateTime, 
            EndDateTime = endDateTime, 
            ShiftDuration = shiftDuration
        };

        var shiftStartDateTime = startDateTime;
        var shiftEndDateTime = startDateTime.AddHours(shiftDuration);
        while (shiftStartDateTime < endDateTime)
        {
            result.Add(new Shift
            {
                ScheduleKey = startDateTime, 
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
        var start = schedule.StartDateTime;
        var end = schedule.EndDateTime;
        var shiftDuration = schedule.ShiftDuration;

        var result = new Schedule { StartDateTime = start, EndDateTime = end, ShiftDuration = shiftDuration };
        result.AddRange(schedule.Select(shift => shift.Copy()));
        return result;
    }
}
