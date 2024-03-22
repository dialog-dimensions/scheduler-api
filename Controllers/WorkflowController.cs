using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.Services.Workflows.Scanners;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : Controller
{
    private readonly IAutoScheduleScanner _scanner;

    public WorkflowController(IAutoScheduleScanner scanner)
    {
        _scanner = scanner;
    }


    [HttpPost("auto/schedule/wake")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Wake()
    {
        await _scanner.Wake();
        return Ok();
    }

    [HttpPost("auto/schedule/terminate")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Terminate()
    {
        _scanner.Terminate();
        return Ok();
    }
}
