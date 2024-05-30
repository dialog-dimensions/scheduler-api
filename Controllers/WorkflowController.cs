using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.Services.Workflows.Jobs;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : Controller
{
    private readonly IJobSchedulingService _jobScheduling;
    private readonly IConfigurationSection _scannerParams;
    private readonly IConfigurationSection _processParams;
    
    public WorkflowController(IJobSchedulingService jobScheduling, IConfiguration configuration)
    {
        _jobScheduling = jobScheduling;
        _processParams = configuration.GetSection("Params:Processes:AutoSchedule");
        _scannerParams = configuration.GetSection("Params:Workflows:AutoScheduleScanner");
    }


    [HttpPost("job/wake")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Wake()
    {
        var jobId = IJobSchedulingService.ScannerJobId;
        var cron = IJobSchedulingService.ScannerCron;
        _jobScheduling.InitializeRecurringJob(jobId, cron);
        return Ok();
    }

    [HttpPost("job/terminate")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Terminate()
    {
        var jobId = IJobSchedulingService.ScannerJobId;
        _jobScheduling.RemoveRecurringJob(jobId);
        return Ok();
    }


    [HttpGet("process/params")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<Dictionary<string, string?>> GetProcessParams()
    {
        return _processParams.GetChildren().ToDictionary(x => x.Key, x => x.Value);
    }
    
    
    [HttpGet("job/params")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<Dictionary<string, string?>> GetScannerParams()
    {
        return _scannerParams.GetChildren().ToDictionary(x => x.Key, x => x.Value);
    }
}
