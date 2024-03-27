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

    [HttpPatch("/api/[controller]/UpdateEmployee/{key:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateEmployee(DateTime key, FlatShiftEmployeeDto flat)
    {
        if (key != flat.ShiftKey)
        {
            return BadRequest(new { Message = "DTO does not match shift key." });
        }

        if (flat.Employee is null)
        {
            return BadRequest(new { Message = "Cannot remove assignment without a replacement." });
        }

        var shift = await _repository.ReadAsync(key);
        if (shift is null)
        {
            return NotFound("Shift not found in database.");
        }

        try
        {
            await _repository.UpdateShiftEmployeeAsync(key, flat.Employee);
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
                flat.ShiftKey, 
                flat.Employee ?? 
                throw new InvalidOperationException($"Flat {flat.ShiftKey} has an empty employee.")
                );
        }
        
        return StatusCode(200);
    }
}