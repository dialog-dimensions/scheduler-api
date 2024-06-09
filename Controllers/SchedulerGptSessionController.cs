using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ChatGPT;
using SchedulerApi.Services.ChatGptServices.Assistants.Interfaces;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulerGptSessionController : Controller
{
    private readonly IGathererServices _gathererService;
    private readonly ISchedulerGptSessionRepository _sessionRepository;

    public SchedulerGptSessionController(
        IGathererServices gathererService, 
        ISchedulerGptSessionRepository sessionRepository
        )
    {
        _gathererService = gathererService;
        _sessionRepository = sessionRepository;
    }
    
    [HttpPost("create-session")]
    [Authorize]
    public async Task<ActionResult<string>> CreateSessionAsync(
        string deskId,
        DateTime scheduleStartDateTime, 
        int employeeId, 
        Dictionary<string, string>? otherInstructions = null)
    {
        return await _gathererService
            .CreateSession(
                (deskId, scheduleStartDateTime),
                employeeId,
                otherInstructions
            );
    }

    // [HttpPost("deliver-to-gpt/{threadId}")]
    // public async Task<IActionResult> DeliverToGpt(string threadId, string message)
    // {
    //     await _schedulerGptService.ProcessIncomingMessage(threadId, message);
    //     return Ok();
    // }

    [HttpGet("{threadId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<Message>>?> GetMessagesAsync(string threadId)
    {
        var session = await _sessionRepository.ReadAsync(threadId);
        if (session is null)
        {
            return null;
        }
        
        return session.Messages.ToList();
    }
}
