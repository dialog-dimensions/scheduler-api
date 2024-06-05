using System.Globalization;
using System.Text;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ImageGenerationServices.Utils;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleHtmlServices;

public class ScheduleHtmlGenerator : IScheduleHtmlGenerator
{
    public string GenerateHorizontal(Schedule schedule)
    {
        var scheduleDays = ScheduleHtmlUtils
            .ShiftsByDate(schedule)
            .Reverse()
            .ToList();

        var startDate = schedule.StartDateTime.Date;
        var endDate = schedule.MaxBy(shift => shift.StartDateTime)!.StartDateTime.Date;
        var maxShifts = scheduleDays.Max(kv => kv.Value.Count);
        
        var html = new StringBuilder();
        html.Append(
            "<html lang=he>" +
            "<head>" +
            "<meta charset=\"UTF-8\">" +
            "<style>" +
            "table { width: 100%; border-collapse: collapse; } " +
            "th, td { border: 1px solid black; padding: 8px; text-align: center; } " +
            "th { background-color: #f2f2f2; }" +
            "</style>" +
            "</head>" +
            "<body>");
        html.Append(
            $"<table>" +
            $"<tr>" +
            $"<th colspan='{scheduleDays.Count}'>{schedule.Desk.Name} {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}</th>" +
            $"</tr>");
        
        html.Append("<tr>");
        foreach (var day in scheduleDays)
        {
            html.Append($"<th>{day.Key.ToString("ddd dd/MM", new CultureInfo("he-IL"))}</th>");
        }
        html.Append("</tr>");
        
        for (var i = 0; i < maxShifts; i++)
        {
            html.Append("<tr>");
            foreach (var (_, day) in scheduleDays)
            {
                if (day.Count > i)
                {
                    var shift = day[i];
                    html.Append($"<td>{shift.StartDateTime.ToString("HH:mm")} - {shift.EndDateTime.ToString("HH:mm")}<br>{shift.Employee!.Name}</td>");
                }
                else
                {
                    html.Append("<td></td>"); // Empty cell if no shift
                }
            }
            html.Append("</tr>");
        }

        html.Append(
            "</table>" +
            "</body>" +
            "</html>");
        
        return html.ToString();
    }
    
    public string GenerateHorizontal(Schedule schedule, Employee employee)
    {
        var scheduleDays = ScheduleHtmlUtils
            .ShiftsByDate(schedule)
            .Reverse()
            .ToList();

        var startDate = schedule.StartDateTime.Date;
        var endDate = schedule.MaxBy(shift => shift.StartDateTime)!.StartDateTime.Date;
        var maxShifts = scheduleDays.Max(kv => kv.Value.Count);

        var highlightEmployeeName = employee.Name;
        
        var html = new StringBuilder();
        html.Append(
            "<html>" +
            "<head>" +
            "<style>" +
            "table { width: 100%; border-collapse: collapse; } " +
            "th, td { border: 1px solid black; padding: 8px; text-align: center; } " +
            "th { background-color: #f2f2f2; }" +
            ".highlight { background-color: #ffff99; }" + // Light yellow background for highlighted cells
            "</style>" +
            "</head>" +
            "<body>");
        html.Append(
            $"<table>" +
            $"<tr>" +
            $"<th colspan='{scheduleDays.Count}'>{schedule.Desk.Name} {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}</th>" +
            $"</tr>");
        
        html.Append("<tr>");
        foreach (var day in scheduleDays)
        {
            html.Append($"<th>{day.Key.ToString("ddd dd/MM", new CultureInfo("he-IL"))}</th>");
        }
        html.Append("</tr>");
        
        for (var i = 0; i < maxShifts; i++)
        {
            html.Append("<tr>");
            foreach (var (_, day) in scheduleDays)
            {
                if (day.Count > i)
                {
                    var shift = day[i];
                    var highlightClass = shift.Employee!.Name == highlightEmployeeName ? " highlight" : "";
                    html.Append($"<td class='{highlightClass}'>{shift.StartDateTime.ToString("HH:mm")} - {shift.EndDateTime.ToString("HH:mm")}<br>{shift.Employee!.Name}</td>");
                }
                else
                {
                    html.Append("<td></td>"); // Empty cell if no shift
                }
            }
            html.Append("</tr>");
        }

        html.Append(
            "</table>" +
            "</body>" +
            "</html>");
        
        return html.ToString(); 
    }
    
    public string GenerateVertical(Schedule schedule, Employee employee)
    {
        var scheduleDays = ScheduleHtmlUtils
            .ShiftsByDate(schedule);

        var startDate = schedule.StartDateTime.Date;
        var endDate = schedule.EndDateTime.Date;
        var lastShiftStartDate = schedule.MaxBy(shift => shift.StartDateTime)!.StartDateTime.Date;

        var highlightEmployeeName = employee.Name;
        
        var html = new StringBuilder();
        html.Append(
            "<html>" +
            "<head>" +
            "<meta charset='UTF-8'>" +
            "<style>" +
            ".container { width: 800px; margin: auto; }" +
            ".day-card { border: 1px solid #ccc; margin-bottom: 10px; padding: 10px; background-color: #f9f9f9; font-size: 16px; font-weight: bold; }" +
            ".day-header { background-color: #f2f2f2; padding: 8px; font-weight: bold; }" +
            ".shift-details { margin-top: 5px; padding: 8px; background-color: #ffffff; border-top: 1px solid #eee; }" +
            ".highlight { background-color: #ffff99; }" +  // Ensuring highlight class is defined
            "</style>" +
            "</head>" +
            "<body>" +
            $"<div class='container'><h2>{schedule.Desk.Name} Schedule: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}</h2>");

        foreach (var day in scheduleDays)
        {
            html.Append(
                $"<div class='day-card'>" +
                $"<div class='day-header'>{day.Key.ToString("dddd, dd/MM", new CultureInfo("he-IL"))}</div>");

            foreach (var shift in day.Value)
            {
                var highlightClass = shift.Employee!.Name.Equals(highlightEmployeeName, StringComparison.OrdinalIgnoreCase) ? "highlight" : "";
                html.Append(
                    $"<div class='shift-details {highlightClass}'>" +
                    $"{shift.StartDateTime.ToString("HH:mm")} - {shift.EndDateTime.ToString("HH:mm")} " +
                    $"{shift.Employee.Name}</div>");
            }

            html.Append("</div>");
        }

        html.Append("</div></body></html>");
        
        return html.ToString();
    }

    public string Generate(Schedule schedule)
    {
        return GenerateHorizontal(schedule);
    }

    public string Generate(Schedule schedule, Employee employee)
    {
        return NewVertical(schedule, employee);
    }

    private string NewVertical(Schedule schedule, Employee employee)
    {
        var days = ScheduleHtmlUtils.ShiftsByDate(schedule);
        var startDate = schedule.StartDateTime.Date;
        var endDate = schedule.EndDateTime.Date;
        var lastShiftStartDate = schedule.LastShift.StartDateTime.Date;
        var deskName = schedule.Desk.Name;
        
        var result = new StringBuilder();
        result.Append(
            "<!DOCTYPE html>" +
            "<html lang=\"he\" dir=\"rtl\">" +
            "<head>" +
            "<meta charset=\"UTF-8\">" +
            "<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">" +
            "<title>Weekly Shift Schedule</title>" +
            "<style>" +
            "body {font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; background-color: #ffffff; width: 400px; padding: 20px; color: #333333; direction: rtl;}" +
            "table {width: 100%; border-collapse: collapse;}" +
            ".day-header {width: 60px; background-color: #e7e7e7; color: #333; text-align: center; border-bottom: 1px solid #000000; font-weight: bold; border-left: 2px solid #000000;}" +
            ".shift {background-color: #f9f9f9; color: #333; text-align: center; padding: 10px; border: 1px solid #dddddd; font-size: 14px;}" +
            ".hour {text-align: center; width: 50px;}" +
            ".highlight {background-color: #ffff99;}" +
            ".odd-day {background-color: #ffffff;}" +
            ".day-ender {border-bottom: 1px solid #000000;}" +
            "h3 {text-align: center; margin: 10px 0;}" +
            "</style>" +
            "</head>" +
            "<body>" +
            $"<h3>{deskName} {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}</h3>" +
            $"<table>"
        );

        foreach (var dayKv in days)
        {
            var shifts = dayKv.Value;
            foreach (var shift in shifts)
            {
                result.Append(BuildShiftString(shift, employee, dayKv));
            }
        }

        result.Append(
            "</table>" +
            "</body>" +
            "</html>");

        return result.ToString();
    }

    private string BuildShiftString(Shift shift, Employee employee, KeyValuePair<DateTime, List<Shift>> day)
    {
        var heb = new CultureInfo("he-IL");
        var result = new StringBuilder();
        
        result.Append("<tr>");
        if (day.Value.Min(s => s.StartDateTime) == shift.StartDateTime)
        {
            result.Append(
                $"<td class=\"day-header\" rowspan={day.Value.Count()}>" +
                $"{day.Key.ToString("ddd", heb)}<br>" +
                $"{day.Key.ToString("dd/MM", heb)}" +
                $"</td>");
        }
        result.Append(ShiftStringCore(isHour: true));
        result.Append(ShiftStringCore(isHour: false));
        result.Append("</tr>");

        return result.ToString();
        
        string ShiftStringCore(bool isHour) =>
            $"<td class=\"shift " +
            $"{(isHour ? "hour " : "")} " +
            $"{(IsHighlight(shift, employee) ? "highlight " : IsOddDay(shift) ? "odd-day " : "")} " +
            $"{(IsDayEnder(shift) ? "day-ender " : "")}\">" +
            $"{(isHour ? shift.StartDateTime.ToString("HH:mm") : shift.Employee!.Name)}" +
            $"</td>";
    }
    
    private bool IsOddDay(Shift shift) => (int)shift.StartDateTime.DayOfWeek % 2 == 0; 
    // the condition is for even because the enum value starts at 0.
    private bool IsDayEnder(Shift shift) => shift.EndDateTime.Date > shift.StartDateTime.Date;
    // Last Shift of A Day are Shifts Which End Day After They Started.
    private bool IsHighlight(Shift shift, Employee employee) => shift.EmployeeId == employee.Id;
    // Shifts Taken by the Employee in Hand.
}
