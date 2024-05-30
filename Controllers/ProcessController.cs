using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs.OrganizationEntities;
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
    public async Task<IActionResult> Run(string deskId, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var process = _serviceProvider.GetRequiredService<IAutoScheduleProcess>();
        await process.Run(deskId, startDateTime, endDateTime, shiftDuration);
        return Ok();
    }
    
    [HttpPost("gpt")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RunGpt(string deskId, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var process = _serviceProvider.GetRequiredService<IGptScheduleProcess>();
        await process.Run(deskId, startDateTime, endDateTime, shiftDuration);
        return Ok();
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
        return Ok();
    }
}
