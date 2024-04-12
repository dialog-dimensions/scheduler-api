using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs.OrganizationEntities;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitController : Controller
{
    private readonly IUnitRepository _repository;

    public UnitController(IUnitRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<UnitDto>> GetUnit(string id)
    {
        var unit = await _repository.ReadAsync(id);
        if (unit is null)
        {
            return NotFound();
        }

        return UnitDto.FromEntity(unit);
    }
    
    [HttpGet("org")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<OrganizationDto>> GetOrganization(string id)
    {
        var unit = await _repository.ReadAsync(id);
        if (unit is null)
        {
            return NotFound();
        }

        var organization = await _repository.GetUnderlyingOrganizationAsync(id);
        if (organization is null)
        {
            throw new InvalidOperationException("Unable to construct organization tree.");
        }
        
        return OrganizationDto.FromEntity(organization);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> PostUnit(UnitDto unit)
    {
        await _repository.CreateAsync(unit.ToEntity());
        return Ok();
    }

    [HttpDelete("{unitId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteUnit(string unitId)
    {
        try
        {
            await _repository.DeleteAsync(unitId);
        }
        catch (KeyNotFoundException)
        {
            return NoContent();
        }

        return Ok();
    }
}
