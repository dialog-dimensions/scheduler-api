using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs.OrganizationEntities;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeskController : Controller
{
    private readonly IDeskRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;

    public DeskController(IDeskRepository repository, IEmployeeRepository employeeRepository)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
    }

    [HttpGet("unit-desks/{unitId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<DeskDto>>> GetUnitDesks(string unitId)
    {
        return (await _repository.ReadAllUnit(unitId)).Select(DeskDto.FromEntity).ToList();
    }

    [HttpGet("{deskId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<DeskDto?>> GetDesk(string deskId)
    {
        var result = await _repository.ReadAsync(deskId);
        if (result is null)
        {
            return NotFound();
        }

        return DeskDto.FromEntity(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> PostDesk(DeskDto desk)
    {
        await _repository.CreateAsync(desk.ToEntity());
        return Ok();
    }

    [HttpDelete("{deskId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteDesk(string deskId)
    {
        try
        {
            await _repository.DeleteAsync(deskId);
        }
        catch (KeyNotFoundException)
        {
            return NoContent();
        }

        return Ok();
    }

    [HttpPost("assign/{deskId}/{employeeId:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AssignToDesk(string deskId, int employeeId)
    {
        var desk = await _repository.ReadAsync(deskId);
        if (desk is null)
        {
            return NotFound("desk not found in database.");
        }

        var employee = await _employeeRepository.ReadAsync(employeeId);
        if (employee is null)
        {
            return NotFound("employee not found in database.");
        }

        await _repository.AddDeskAssignment(desk, employee);
        return Ok();
    }

    [HttpDelete("assign/{deskId}/{employeeId:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> RemoveAssignmentFromDesk(string deskId, int employeeId)
    {
        var desk = await _repository.ReadAsync(deskId);
        if (desk is null)
        {
            return NotFound("desk not found in database.");
        }

        var employee = await _employeeRepository.ReadAsync(employeeId);
        if (employee is null)
        {
            return NotFound("employee not found in database.");
        }

        try
        {
            await _repository.RemoveDeskAssignment(desk, employee);
        }
        catch (KeyNotFoundException)
        {
            return NoContent();
        }

        return Ok();
    }
}
