using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.Services.Workflows.Jobs.Classes;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : Controller
{
    private readonly IRecurringJobManager _recurringJobManager;

    public JobController(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    private string GetScannerJobId(string deskId) => $"{deskId}_scanner";

    [HttpPost("initialize-scanner/{deskId}")]
    [Authorize]
    public IActionResult InitializeScanner(string deskId, string cronExpression)
    {
        _recurringJobManager.AddOrUpdate<GptProcessInitiationJob>(
            GetScannerJobId(deskId),
            job => job.Execute(deskId),
            cronExpression
        );
        
        return Ok();
    }

    [HttpPost("remove-scanner/{deskId}")]
    [Authorize]
    public IActionResult RemoveScanner(string deskId)
    {
        var jobId = GetScannerJobId(deskId);
        _recurringJobManager.RemoveIfExists(jobId);
        return NoContent();
    }
}
