using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL;
using SchedulerApi.Models.DTOs;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ShiftSwapController : Controller
{
    private readonly ApiDbContext _context;

    public ShiftSwapController(ApiDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<ShiftSwapDto>>> GetSwaps()
    {
        var swaps = await _context.Swaps.ToListAsync();
        return swaps.Select(ShiftSwapDto.FromEntity).ToList();
    }
}
