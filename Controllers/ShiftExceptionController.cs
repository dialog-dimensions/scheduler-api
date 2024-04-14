using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.WhatsAppClient.Twilio;
using SchedulerApi.Services.Workflows.Processes.Classes;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class 
    ShiftExceptionController : Controller
{
    private readonly ApiDbContext _context;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAutoScheduleProcessRepository _processRepository;
    private readonly ITwilioServices _twilio;
    private readonly UserManager<IdentityUser> _userManager;

    public ShiftExceptionController(ApiDbContext context, IScheduleRepository scheduleRepository, ITwilioServices twilio
    , IAutoScheduleProcessRepository processRepository, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _scheduleRepository = scheduleRepository;
        _twilio = twilio;
        _processRepository = processRepository;
        _userManager = userManager;
    }
    
    
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<ShiftExceptionDto>>> GetExceptions()
    {
        var exceptions = await _context.Exceptions
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .ToListAsync();
        
        var dtos = exceptions.Select(ShiftExceptionDto.FromEntity);
        return dtos.ToList();
    }

    
    [HttpGet("{employeeId:int}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ShiftExceptionDto>>> GetEmployeeExceptions(int employeeId)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var parseUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);

        if (!parseUserId || role is not ("Employee" or "Manager" or "Admin"))
            return Unauthorized(new { Message = "Invalid token." });

        if (role is "Employee" & userId != employeeId) return Forbid();

        var exceptions = await _context.Exceptions
            .Where(ex => ex.EmployeeId == employeeId)
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .ToListAsync();
        
        var result = exceptions.Select(ShiftExceptionDto.FromEntity);
        return result.ToList();
    }

    [HttpGet("{deskId}/{scheduleStart:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<IEnumerable<ShiftExceptionDto>>> GetScheduleExceptions(string deskId, DateTime scheduleStart)
    {
        var exceptions = await _context.Exceptions
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(ex => ex.Shift.ScheduleStartDateTime == scheduleStart)
            .Where(ex => ex.Shift.Desk.Id == deskId)
            .ToListAsync();
        var result = exceptions.Select(ShiftExceptionDto.FromEntity);
        return result.ToList();
    }
    
    [HttpGet("{deskId}/{scheduleStart:datetime}/{employeeId:int}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<ShiftExceptionDto>>> GetScheduleEmployeeExceptions(string deskId, DateTime scheduleStart, int employeeId)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var parseUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);

        if (!parseUserId || role is not ("Employee" or "Manager" or "Admin"))
            return Unauthorized(new { Message = "Invalid token." });

        if (role is "Employee" & userId != employeeId) return Forbid();
        
        var exceptions = await _context.Exceptions
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(ex => ex.Shift.ScheduleStartDateTime == scheduleStart)
            .Where(ex => ex.Shift.Desk.Id == deskId)
            .Where(ex => ex.EmployeeId == employeeId)
            .ToListAsync();
        var result = exceptions.Select(ShiftExceptionDto.FromEntity);
        return result.ToList();
    }

    
    // [HttpGet("{key}")]
    // [Authorize]
    // public async Task<ActionResult<ShiftExceptionDto>> GetException((DateTime, int) key)
    // {
    //     var role = User.FindFirst(ClaimTypes.Role)?.Value;
    //     var parseUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
    //
    //     if (!parseUserId || role is not ("Employee" or "Manager" or "Admin"))
    //         return Unauthorized(new { Message = "Invalid token." });
    //
    //     var exception = await _context.Exceptions.FindAsync(key);
    //     if (exception is null) return NotFound();
    //     
    //     if (role is "Employee" & exception.EmployeeId != userId) return Forbid();
    //
    //     return ShiftExceptionDto.FromEntity(exception);
    // }

    
    // [HttpPost]
    // [Authorize]
    // public async Task<IActionResult> PostException(ShiftExceptionDto exception)
    // {
    //     var parseUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
    //     if (!parseUserId)
    //     {
    //         return Unauthorized(new { Message = "Invalid token." });
    //     }
    //     
    //     var role = User.FindFirst(ClaimTypes.Role)?.Value;
    //     if (role is not ("Employee" or "Manager" or "Admin"))
    //     {
    //         return Unauthorized();
    //     }
    //
    //     if (role is "Employee" & userId != exception.EmployeeId)
    //     {
    //         return Forbid();
    //     }
    //
    //     var employee = await _context.Employees.FindAsync(exception.EmployeeId);
    //     if (employee is null)
    //     {
    //         return NotFound(new { Message = "Employee not found in database." });
    //     }
    //     
    //     var shift = await _context.Shifts.FindAsync(exception.ShiftKey);
    //     if (shift is null)
    //     {
    //         return NotFound(new { Message = "Shift not found in database." });
    //     }
    //     
    //     var exceptionAlreadyExists = await _context.Exceptions.FindAsync(exception.ShiftKey, exception.EmployeeId) is not null;
    //     if (exceptionAlreadyExists)
    //     {
    //         return BadRequest("Exception already exists for employee for shift.");
    //     }
    //
    //     _context.Exceptions.Add(exception.ToEntity());
    //     
    //     var result = await _context.SaveChangesAsync();
    //     return result == 0
    //         ? Problem("Unable to save exception into the database.")
    //         : Ok(); //CreatedAtAction(nameof(GetException), new { key }, exception); 
    // }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostExceptions(List<ShiftExceptionDto> exceptions)
    {
        string? deskId = null;
        DateTime? scheduleStartDateTime = null;
        
        var parseUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
        if (!parseUserId)
        {
            return Unauthorized(new { Message = "Invalid token." });
        }
        
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role is not ("Employee" or "Manager" or "Admin"))
        {
            return Unauthorized();
        }

        if (role is "Employee" & exceptions.Any(exception => userId != exception.EmployeeId))
        {
            return Forbid();
        }
        
        var employee = await _context.Employees.FindAsync(userId);
        if (employee is null)
        {
            return NotFound(new { Message = "Employee not found in database." });
        }
        
        // FOR SENDING ACK BACK TO THE USER
        bool sendAck = false;
        if (employee.Active)
        {
            var arbitraryShiftStartDateTime = exceptions.First().ShiftStartDateTime;
            var arbitraryShiftDeskId = exceptions.First().Desk.Id;
            var arbitraryShift = await _context.Shifts
                .Include(shift => shift.Desk)
                .FirstOrDefaultAsync(
                    shift => shift.StartDateTime == arbitraryShiftStartDateTime && 
                             shift.Desk.Id == arbitraryShiftDeskId
                             );
            if (arbitraryShift is not null)
            {
                deskId = arbitraryShift.Desk.Id;
                scheduleStartDateTime = arbitraryShift.ScheduleStartDateTime;
                var firstSubmission = !await _context.Exceptions
                    .Include(ex => ex.Shift)
                    .ThenInclude(shift => shift.Desk)
                    .ThenInclude(desk => desk.Unit)
                    .Include(ex => ex.Desk)
                    .ThenInclude(desk => desk.Unit)
                    .Include(ex => ex.Employee)
                    .ThenInclude(emp => emp.Unit)
                    .Where(ex => ex.Shift.ScheduleStartDateTime == scheduleStartDateTime)
                    .Where(ex => ex.Shift.Desk.Id == deskId)
                    .AnyAsync(ex => ex.EmployeeId == userId);
                sendAck = firstSubmission;
            }
        }
        
        foreach (var exception in exceptions)
        {
            var shift = await _context.Shifts.FindAsync(exception.Desk.Id, exception.ShiftStartDateTime);
            if (shift is null)
            {
                return NotFound(new { Message = "Shift not found in database." });
            }
        
            var exceptionAlreadyExists = await _context.Exceptions.FindAsync(exception.Desk.Id, exception.ShiftStartDateTime, exception.EmployeeId) is not null;
            if (exceptionAlreadyExists)
            {
                return BadRequest("Exception already exists for employee for shift.");
            }

            _context.Exceptions.Add(exception.ToEntity());
        }

        if (await _context.SaveChangesAsync() != exceptions.Count)
        {
            return Problem("Unable to save exception into the database.");
        }

        if (sendAck)
        {
            Console.WriteLine($"{DateTime.Now:dd-MM HH:mm:ss} Sending file ack back to user...");
            var runningProcess = await _processRepository.ReadRunningAsync(deskId!, scheduleStartDateTime!.Value);
            if (runningProcess is not null)
            {
                var fileWindowEnd = runningProcess.FileWindowEnd;
                var publishDateTime = runningProcess.PublishDateTime;
                var desk = runningProcess.Desk;
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user is not null)
                {
                    await _twilio.TriggerAckFileFlow(user.PhoneNumber!, desk, fileWindowEnd, publishDateTime);
                }
            }
                
        }

        return Ok();
    }

    [HttpPut("{deskId}/{scheduleStart:datetime}/{employeeId:int}")]
    [Authorize]
    public async Task<IActionResult> PutExceptions(string deskId ,DateTime scheduleStart, int employeeId, List<ShiftExceptionDto> exceptions)
    {
        var parseUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
        if (!parseUserId)
        {
            return Unauthorized(new { Message = "Invalid token." });
        }
        
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role is not ("Employee" or "Manager" or "Admin"))
        {
            return Unauthorized();
        }

        if (role is "Employee" & exceptions.Any(exception => userId != exception.EmployeeId))
        {
            return Forbid();
        }
        
        if (role is "Employee" & employeeId != userId)
        {
            return Forbid();
        }
        
        var employee = await _context.Employees.FindAsync(userId);
        if (employee is null)
        {
            return NotFound(new { Message = "Employee not found in database." });
        }

        var schedule = await _scheduleRepository.ReadAsync((deskId, scheduleStart));
        if (schedule is null)
        {
            return NotFound(new { Message = "Schedule not found in database." });
        }
        
        var scheduleEmployeeExceptions = await _context.Exceptions
            .Include(ex => ex.Shift)
            .ThenInclude(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(ex => ex.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(ex => ex.EmployeeId == employeeId)
            .Where(ex => ex.Shift.ScheduleStartDateTime == scheduleStart)
            .Where(ex => ex.Shift.Desk.Id == deskId)
            .ToListAsync();
        
        _context.Exceptions.RemoveRange(scheduleEmployeeExceptions);
        _context.Exceptions.AddRange(exceptions.Select(dto => dto.ToEntity()));
        
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    
    [HttpDelete("{deskId}/{shiftStart:datetime}/{employeeId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteException(string deskId, DateTime shiftStart, int employeeId)
    {
        var parseUserId = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
        if (!parseUserId) return Unauthorized(new { Message = "Invalid token." });

        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role is not ("Employee" or "Manager" or "Admin")) return Unauthorized(new { Message = "Invalid token." });

        var exception = await _context.Exceptions.FindAsync(deskId, shiftStart, employeeId);
        if (exception is null) return NotFound(new { Message = "Exception not found in database." });

        if (exception.EmployeeId != userId) return Forbid();

        var shift = await _context.Shifts.FindAsync(exception.DeskId, exception.ShiftStartDateTime);
        if (shift!.ScheduleStartDateTime < DateTime.Now) 
            return UnprocessableEntity("Unable to delete non-future exceptions.");

        _context.Exceptions.Remove(exception);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}