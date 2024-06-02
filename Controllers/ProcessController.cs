using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Interfaces;

namespace SchedulerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProcessController : Controller
{
    private readonly IServiceProvider _serviceProvider;

    public ProcessController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<int>> Run(string deskId, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var process = _serviceProvider.GetRequiredService<IAutoScheduleProcess>();
        var id = await process.Run(deskId, startDateTime, endDateTime, shiftDuration);
        return id;
    }
    
    [HttpPost("gpt")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<int>> RunGpt(string deskId, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var process = _serviceProvider.GetRequiredService<IGptScheduleProcess>();
        var id = await process.Run(deskId, startDateTime, endDateTime, shiftDuration);
        return id;
    }

    [HttpPost("runScheduler")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ScheduleResults>> RunScheduler(string deskId, DateTime scheduleStartDateTime)
    {
        var scheduleEngine = _serviceProvider.GetRequiredService<IScheduler>();
        var scheduleRepository = _serviceProvider.GetRequiredService<IScheduleRepository>();
        var schedule = await scheduleRepository.ReadAsync((deskId, scheduleStartDateTime));
        var results = await scheduleEngine.RunAsync(schedule!);
        await scheduleRepository.AssignEmployees(deskId, scheduleStartDateTime, results.CompleteSchedule);
        return results;
    }
}
