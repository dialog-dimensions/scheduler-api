using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Run(DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var process = _serviceProvider.GetRequiredService<IAutoScheduleProcess>();
        await process.Run(startDateTime, endDateTime, shiftDuration);
        return Ok();
    }
}
