﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs;
using SchedulerApi.Services.ChatGptServices.Assistants.Interfaces;
using SchedulerApi.Services.WhatsAppClient.Twilio;
using Twilio.AspNet.Common;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TwilioController : Controller
{
    private readonly ITwilioServices _twilioServices;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IDeskRepository _deskRepository;
    private readonly ISchedulerGptSessionRepository _gptSessionRepository;
    private readonly IGathererServices _gptGathererServices;
    private readonly IManagerSupportServices _managerSupport;

    public TwilioController(
        ITwilioServices twilioServices, 
        IEmployeeRepository employeeRepository, 
        UserManager<IdentityUser> userManager, 
        IScheduleRepository scheduleRepository, 
        IDeskRepository deskRepository,
        ISchedulerGptSessionRepository gptSessionRepository,
        IGathererServices gptServices, IManagerSupportServices managerSupport)
    {
        _twilioServices = twilioServices;
        _employeeRepository = employeeRepository;
        _userManager = userManager;
        _scheduleRepository = scheduleRepository;
        _deskRepository = deskRepository;
        _gptSessionRepository = gptSessionRepository;
        _gptGathererServices = gptServices;
        _managerSupport = managerSupport;
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
    
    [HttpPost("PublishShifts/{deskId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> PublishShifts(string deskId)
    {
        var desk = await _deskRepository.ReadAsync(deskId);
        if (desk is null)
        {
            return NotFound("desk not found in database.");
        }
        
        var activeEmployees = await _employeeRepository.ReadAllActiveAsync(deskId);
        var nearestSchedule = await _scheduleRepository.ReadNextAsync(deskId);
        if (nearestSchedule is null)
        {
            return NotFound("no upcoming schedule for requested desk.");
        }
        
        try
        {
            foreach (var employee in activeEmployees)
            {
                var user = await _userManager.FindByIdAsync(employee.Id.ToString());
                if (user is null)
                {
                    continue;
                }

                await _twilioServices.TriggerPublishShiftsFlow(user.PhoneNumber!, desk, employee.Name,
                    nearestSchedule.StartDateTime, nearestSchedule.EndDateTime);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromForm] SmsRequest twilioMessage)
    {
        var from = "0" + twilioMessage.From.Substring(twilioMessage.From.Length - 9, 9);
        
        var user = await _userManager.Users.Where(u => u.PhoneNumber == from).FirstOrDefaultAsync();
        if (user is null)
        {
            return NotFound("user not found");
        }

        var employee = await _employeeRepository.ReadAsync(int.Parse(user.Id));
        if (employee is null)
        {
            return NotFound("employee not found");
        }

        if (employee.Role == "Employee")
        {
            var session = await _gptSessionRepository.FindActiveByEmployeeIdAsync(employee.Id);
            if (session is null)
            {
                return NotFound("no session for employee.");
            }

            await _gptGathererServices.ProcessIncomingMessage(session.ThreadId, twilioMessage.Body);
            return Ok();
        }
        
        else if (employee.Role == "Manager")
        {
            await _managerSupport.ProcessIncomingMessage(employee, twilioMessage.Body);
        }

        return NoContent();
    }

    [HttpPost("publish-media")]
    [Authorize]
    public async Task<IActionResult> PublishShiftsMediaAsync(string deskId, DateTime scheduleStartDateTime)
    {
        var data = await _scheduleRepository.GetScheduleData(deskId, scheduleStartDateTime);
        var schedule = await _scheduleRepository.ReadAsync((deskId, scheduleStartDateTime));
        data.Schedule = schedule!;
        foreach (var employee in data.Employees)
        {
            var user = await _userManager.FindByIdAsync(employee.Id.ToString());
            await _twilioServices.TriggerPublishShiftsMediaFlow(user!.PhoneNumber!, employee.Name, schedule!, employee);
        }

        return Ok();
    }
}
