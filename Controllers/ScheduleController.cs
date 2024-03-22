using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs;
using SchedulerApi.Models.DTOs.ScheduleEngineModels;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ScheduleController : Controller
{
    private readonly IScheduleRepository _repository;
    private readonly IShiftRepository _shiftRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IScheduler _scheduler;

    public ScheduleController(IScheduleRepository repository, IShiftRepository shiftRepository, 
        IEmployeeRepository employeeRepository, IScheduler scheduler)
    {
        _repository = repository;
        _shiftRepository = shiftRepository;
        _employeeRepository = employeeRepository;
        _scheduler = scheduler;
    }


    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FlatScheduleDto>>> GetFlatSchedules()
    {
        var schedules = await _repository.ReadAllAsync();
        var result = schedules.Select(FlatScheduleDto.FromEntity);
        return result.ToList();
    }


    [HttpGet("{key:datetime}")]
    [Authorize]
    public async Task<ActionResult<ScheduleDto?>> GetSchedule(DateTime key)
    {
        var schedule = await _repository.ReadAsync(key);
        return schedule is null ? null : ScheduleDto.FromEntity(schedule);
    }


    [HttpPost("/api/[controller]/Auto/{key:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ScheduleResultsDto>> RunScheduler(DateTime key, ScheduleDto schedule)
    {
        if (schedule.StartDateTime != key)
        {
            return BadRequest("Entity-Key mismatch.");
        }
        var scheduleResults = await _scheduler.RunAsync(schedule.ToEntity());
        return ScheduleResultsDto.FromEntity(scheduleResults);
    }


    [HttpGet("/api/[controller]/Latest")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ScheduleDto?>> GetLatestSchedule()
    {
        var schedule = await _repository.ReadLatestAsync();
        return schedule is null ? null : ScheduleDto.FromEntity(schedule);
    }
    
    [HttpGet("/api/[controller]/NearestIncomplete")]
    [Authorize]
    public async Task<ActionResult<FlatScheduleDto?>> GetNearestIncompleteSchedule()
    {
        var schedule = await _repository.ReadNearestIncomplete();
        return schedule is null ? null : FlatScheduleDto.FromEntity(schedule);
    }
    
    
    [HttpGet("/api/[controller]/Data/{key:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ScheduleDataDto>> GetScheduleData(DateTime key)
    {
        var schedule = await _repository.ReadAsync(key);
        if (schedule is null)
        {
            return NotFound();
        }
        
        var data = await _repository.GetScheduleData(key);
        data.Schedule = schedule;
        return ScheduleDataDto.FromEntity(data);
    }
    
    [HttpPost("/api/[controller]/Report/{key:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ScheduleReportDto>> GetScheduleReport(DateTime key, ScheduleDto schedule)
    {
        if (key != schedule.StartDateTime)
        {
            return BadRequest();
        }
    
        var report = await _repository.GetScheduleReport(schedule.ToEntity());
        return ScheduleReportDto.FromEntity(report);
    }

    [HttpGet("Current")]
    [Authorize]
    public async Task<ScheduleDto?> GetCurrentSchedule()
    {
        var result = await _repository.ReadCurrentAsync();
        return result is null ? null : ScheduleDto.FromEntity(result);
    }
    
    [HttpGet("Next")]
    [Authorize]
    public async Task<ScheduleDto?> GetNextSchedule()
    {
        var result = await _repository.ReadNextAsync();
        return result is null ? null : ScheduleDto.FromEntity(result);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> PostSchedule(ScheduleDto schedule)
    {
        await _repository.CreateAsync(schedule.ToEntity());
        //return CreatedAtAction(nameof(GetSchedule), new { scheduleKey = schedule.StartDateTime }, schedule);
        return Ok();
    }

    [HttpPatch("Assign/{key:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AssignEmployees(DateTime key, ScheduleDto schedule)
    {
        if (schedule.StartDateTime != key)
        {
            return BadRequest("schedule-key mismatch");
        }
        
        var assignedSchedule = schedule.ToEntity();
        await _repository.AssignEmployees(key, assignedSchedule);
        return Ok();
    }
    

    // [HttpPatch("/api/[controller]/Assign/{key:datetime}")]
    // [Authorize(Roles = "Admin,Manager")]
    // public async Task<ActionResult<IEnumerable<DateTime>>> AssignEmployeesBackup(DateTime key,
    //     ScheduleDto dto)
    // {
    //     if (key != dto.MinBy(shiftDto => shiftDto.StartDateTime)?.StartDateTime) 
    //     {
    //         return BadRequest();
    //     }
    //
    //     try
    //     {
    //         var faultyShifts = (await _repository.UpdateAsync(dto)).ToList();
    //         if (faultyShifts.Count != 0)
    //         {
    //             return UnprocessableEntity(new
    //             {
    //                 Message = "Foreign shifts in dto.",
    //                 Foreigns = faultyShifts
    //             });
    //         }
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         return NotFound(new { ex.Message });
    //     }
    //
    //     return NoContent();
    // }
}
