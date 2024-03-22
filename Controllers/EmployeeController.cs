using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class EmployeeController : Controller
{
    private readonly IEmployeeRepository _repository;

    public EmployeeController(IEmployeeRepository repository)
    {
        _repository = repository;
    }


    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployeesAsync()
    {
        var employees = await _repository.ReadAllAsync();
        var result = employees.Select(EmployeeDto.FromEntity);
        return result.ToList();
    }
    
    [HttpGet("Active")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetActiveEmployeesAsync()
    {
        var employees = await _repository.ReadAllActiveAsync();
        var result = employees.Select(EmployeeDto.FromEntity);
        return result.ToList();
    }
    
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeAsync(int id)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized(new { Message = "Invalid token." });

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole is not ("Employee" or "Manager" or "Admin"))
            return Unauthorized(new { Message = "Invalid token." });

        if (userRole is "Employee" & userId != id) return Forbid();

        var employee = await _repository.ReadAsync(id);
        
        return employee is null 
            ? NotFound(new { Message = "Employee not found in database.", Id = id }) 
            : EmployeeDto.FromEntity(employee);
    }
    
    [HttpGet("Assigned")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAssignedEmployeesAsync()
    {
        var employees = await _repository.ReadAllAssignedAsync();
        var result = employees.Select(EmployeeDto.FromEntity);
        return result.ToList();
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> PostEmployeeAsync(EmployeeDto employee)
    {
        if (await _repository.ReadAsync(employee.Id) is not null)
        {
            return StatusCode(409, new { Message = "Employee already exists." });
        }
        
        await _repository.CreateAsync(employee.ToEntity());
        //return CreatedAtAction("GetEmployeeAsync", new { id = employee.Id }, employee);
        return Ok();
    }
    
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> PutEmployeeAsync(int id, EmployeeDto employee)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            return Unauthorized("Invalid User ID.");
        };
        
        
        if (id != employee.Id)
        {
            return BadRequest();
        }

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole is "Employee" && userId != id)
        {
            return Forbid();
        }

        try
        {
            await _repository.UpdateAsync(employee.ToEntity());
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteEmployeeAsync(int id)
    {
        try
        {
            await _repository.DeleteAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Employee not found in database.");
        }
        
        return Ok();
    }
}