﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ShiftController : Controller
{
    private readonly IShiftRepository _repository;

    public ShiftController(IShiftRepository repository)
    {
        _repository = repository;
    }

    [HttpPatch("/api/[controller]/UpdateEmployee/{deskId}/{start:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateEmployee(string deskId, DateTime start, FlatShiftEmployeeDto flat)
    {
        if (start != flat.ShiftStart || deskId != flat.Desk.Id)
        {
            return BadRequest(new { Message = "DTO does not match shift key." });
        }

        if (flat.Employee is null)
        {
            return BadRequest(new { Message = "Cannot remove assignment without a replacement." });
        }

        var shift = await _repository.ReadAsync((deskId, start));
        if (shift is null)
        {
            return NotFound("Shift not found in database.");
        }

        try
        {
            await _repository.UpdateShiftEmployeeAsync(deskId, start, flat.Employee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

        return StatusCode(200);
    }


    [HttpPatch("/api/[controller]/UpdateEmployees")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateEmployees(List<FlatShiftEmployeeDto> flats)
    {
        if (flats.Any(flat => flat.Employee is null))
        {
            return BadRequest("One or more of the updates contains an empty employee id.");
        }

        foreach (var flat in flats)
        {
            await _repository.UpdateShiftEmployeeAsync(
                flat.Desk.Id,
                flat.ShiftStart, 
                flat.Employee ?? 
                throw new InvalidOperationException($"Flat {flat.Desk.Id}-{flat.ShiftStart} has an empty employee.")
                );
        }
        
        return StatusCode(200);
    }

    [HttpGet("{employeeId:int}/{from:datetime}/{to:datetime}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ShiftDto>>> GetEmployeeShiftsByRange(int employeeId, DateTime from, 
        DateTime to) => 
        (await _repository.GetEmployeeShiftsByRange(employeeId, from, to))
        .Select(ShiftDto.FromEntity)
        .ToList();


    [HttpGet("employeeId:int")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ShiftDto>>> GetEmployeeShifts(int employeeId) =>
        (await _repository.GetEmployeeShifts(employeeId))
        .Select(ShiftDto.FromEntity)
        .ToList();
    
    [HttpGet("desk-shifts/{deskId}/{from:datetime}/{to:datetime}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ShiftDto>>> GetDeskShiftsByRange(string deskId, DateTime from,
        DateTime to) => (await _repository.GetDeskShiftsByRange(deskId, from, to))
        .Select(ShiftDto.FromEntity)
        .ToList();

    [HttpGet("desk-shifts/{deskId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ShiftDto>>> GetDeskShifts(string deskId) =>
        (await _repository.GetDeskShifts(deskId))
        .Select(ShiftDto.FromEntity)
        .ToList();
}
