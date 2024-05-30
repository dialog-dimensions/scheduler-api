using System.Globalization;
using System.Text;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ImageGenerationServices.Utils;

namespace SchedulerApi.Services.ImageGenerationServices.ScheduleToHtmlTable;

public class ScheduleHtmlTableGenerator : IScheduleHtmlTableGenerator
{
    public string Generate(Schedule schedule)
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
            "<html>" +
            "<head>" +
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
    
    public string Generate(Schedule schedule, Employee employee)
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
}