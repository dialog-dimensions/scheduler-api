using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.Services.Workflows.Processes.Interfaces;

namespace SchedulerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProcessController : Controller
{
    private readonly IAutoScheduleProcess _scheduleProcess;

    public ProcessController(IAutoScheduleProcess scheduleProcess)
    {
        _scheduleProcess = scheduleProcess;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Run(DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        await _scheduleProcess.Run(startDateTime, endDateTime, shiftDuration);
        return Ok();
    }
}
