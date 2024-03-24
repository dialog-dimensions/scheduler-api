using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.Services.Workflows.Scanners;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : Controller
{
    private readonly IAutoScheduleScanner _scanner;
    private readonly IConfigurationSection _scannerParams;
    private readonly IConfigurationSection _processParams;
    
    public WorkflowController(IAutoScheduleScanner scanner, IConfiguration configuration)
    {
        _scanner = scanner;
        _processParams = configuration.GetSection("Params:Processes:AutoSchedule");
        _scannerParams = configuration.GetSection("Params:Workflows:AutoScheduleScanner");
    }


    [HttpPost("scanner/wake")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Wake()
    {
        await _scanner.Wake();
        return Ok();
    }

    [HttpPost("scanner/terminate")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult Terminate()
    {
        _scanner.Terminate();
        return Ok();
    }

    [HttpGet("scanner/status")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<bool> ScannerStatus()
    {
        return _scanner.ShouldRun;
    }
    
    [HttpGet("process/status")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<string> ProcessStatus()
    {
        var processInProgress = _scanner.ProcessInProgress;
        if (processInProgress is null)
        {
            return "No Process";
        }
    
        return processInProgress.Status.ToString();
    }
    
    [HttpGet("process/currentStep")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<string?> ProcessCurrentStep()
    {
        var processInProgress = _scanner.ProcessInProgress;
        if (processInProgress is null)
        {
            return NoContent();
        }
    
        return processInProgress.Status == TaskStatus.Running ? 
            processInProgress.CurrentStep!.Task.Method.Name : 
            processInProgress.Status.ToString();
    }
    
    [HttpGet("process/strategy")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<IEnumerable<string>?> ProcessStrategy()
    {
        var processInProgress = _scanner.ProcessInProgress;
        if (processInProgress is null)
        {
            return NoContent();
        }
    
        return processInProgress.Strategy.StepsInStrategyToString().ToList();
    }
    
    [HttpGet("process/startDateTime")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<DateTime?> ProcessStart()
    {
        var processInProgress = _scanner.ProcessInProgress;
        if (processInProgress is null)
        {
            return NoContent();
        }
    
        return processInProgress.ProcessStart;
    }
    
    [HttpGet("process/fileWindowEnd")]
    [Authorize]
    public ActionResult<DateTime?> ProcessFileWindowEnd()
    {
        var processInProgress = _scanner.ProcessInProgress;
        if (processInProgress is null)
        {
            return NoContent();
        }
    
        return processInProgress.FileWindowEnd;
    }
    
    [HttpGet("process/publishDateTime")]
    [Authorize]
    public ActionResult<DateTime?> ProcessPublishDateTime()
    {
        var processInProgress = _scanner.ProcessInProgress;
        if (processInProgress is null)
        {
            return NoContent();
        }
    
        return processInProgress.PublishDateTime;
    }

    [HttpGet("process/params")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<Dictionary<string, string?>> GetProcessParams()
    {
        return _processParams.GetChildren().ToDictionary(x => x.Key, x => x.Value);
    }
    
    
    [HttpGet("scanner/params")]
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult<Dictionary<string, string?>> GetScannerParams()
    {
        return _scannerParams.GetChildren().ToDictionary(x => x.Key, x => x.Value);
    }
}
