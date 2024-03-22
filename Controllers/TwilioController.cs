using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs;
using SchedulerApi.Services.WhatsAppClient.Twilio;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TwilioController : Controller
{
    private readonly ITwilioServices _twilioServices;
    private readonly IEmployeeRepository _employeeRepository;

    public TwilioController(ITwilioServices twilioServices, IEmployeeRepository employeeRepository)
    {
        _twilioServices = twilioServices;
        _employeeRepository = employeeRepository;
    }
    
    // [HttpPost("{phoneNumber}")]
    // public async Task<IActionResult> PostCallToFile(string userName, string userId, string phoneNumber, DateTime scheduleStartDateTime)
    // {
    //
    //     try
    //     {
    //         await _twilioServices.TriggerCallToFileFlow(userName, userId, phoneNumber, scheduleStartDateTime);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine("Trigger exception input flow failure.");
    //         Console.WriteLine(ex.Message);
    //         return Problem(ex.Message);
    //     }
    //
    //     return Ok();
    // }
    //
    // [HttpPost("/api/[controller]/Test")]
    // public async Task<IActionResult> PostTest()
    // {
    //     try
    //     {
    //         await _twilioServices.TriggerTestFlow();
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine("Trigger test input flow failure.");
    //         Console.WriteLine(ex.Message);
    //         return Problem(ex.Message);
    //     }
    //
    //     return Ok();
    // }

    [HttpPost("CallToRegister")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CallToRegister(CallToRegisterDto dto)
    {
        var id = dto.Id;
        var phoneNumber = dto.PhoneNumber;
        
        var employee = await _employeeRepository.ReadAsync(id);
        if (employee is null)
        {
            return NotFound();
        }

        try
        {
            await _twilioServices.TriggerCallToRegisterFlow(employee.Name, id.ToString(), phoneNumber);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
