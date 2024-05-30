using SchedulerApi.Models.Entities;

namespace SchedulerApi.Services.ImageGenerationServices.Utils;

public static class ScheduleHtmlUtils
{
    public static Dictionary<DateTime, List<Shift>> ShiftsByDate(IEnumerable<Shift> shifts) =>
        shifts
            .GroupBy(shift => shift.StartDateTime.Date)
            .ToDictionary(
                group => group.Key,
                group => group.ToList()
            );
}