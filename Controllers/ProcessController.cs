using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.Models.DTOs.OrganizationEntities;
using SchedulerApi.Models.Organization;
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
    public async Task<IActionResult> Run(DeskDto desk, DateTime startDateTime, DateTime endDateTime, int shiftDuration)
    {
        var process = _serviceProvider.GetRequiredService<IAutoScheduleProcess>();
        await process.Run(desk.ToEntity(), startDateTime, endDateTime, shiftDuration);
        return Ok();
    }
}
