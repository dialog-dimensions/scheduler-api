using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.DTOs;
using SchedulerApi.Models.DTOs.ScheduleEngineModels;
using SchedulerApi.Services.ImageGenerationServices.ScheduleImageServices;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ScheduleController : Controller
{
    private readonly IScheduleRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IScheduler _scheduler;
    private readonly IScheduleImagePublisher _imagePublisher;

    public ScheduleController(
        IScheduleRepository repository, 
        IScheduler scheduler,
        IEmployeeRepository employeeRepository, 
        IScheduleImagePublisher imagePublisher)
    {
        _repository = repository;
        _scheduler = scheduler;
        _imagePublisher = imagePublisher;
        _employeeRepository = employeeRepository;
    }


    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FlatScheduleDto>>> GetFlatSchedules()
    {
        var schedules = await _repository.ReadAllAsync();
        var result = schedules.Select(FlatScheduleDto.FromEntity);
        return result.ToList();
    }


    [HttpGet("{deskId}/{start:datetime}")]
    [Authorize]
    public async Task<ActionResult<ScheduleDto?>> GetSchedule(string deskId, DateTime start)
    {
        var schedule = await _repository.ReadAsync((deskId, start));
        return schedule is null ? null : ScheduleDto.FromEntity(schedule);
    }


    [HttpPost("/api/[controller]/Auto/{deskId}/{start:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ScheduleResultsDto>> RunScheduler(string deskId, DateTime start, ScheduleDto schedule)
    {
        if (schedule.StartDateTime != start || schedule.Desk.Id != deskId)
        {
            return BadRequest("Entity-Key mismatch.");
        }
        var scheduleResults = await _scheduler.RunAsync(schedule.ToEntity());
        return ScheduleResultsDto.FromEntity(scheduleResults);
    }


    [HttpGet("/api/[controller]/{deskId}/Latest")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ScheduleDto?>> GetLatestSchedule(string deskId)
    {
        var schedule = await _repository.ReadLatestAsync(deskId);
        return schedule is null ? null : ScheduleDto.FromEntity(schedule);
    }
    
    [HttpGet("/api/[controller]/{deskId}/NearestIncomplete")]
    [Authorize]
    public async Task<ActionResult<FlatScheduleDto?>> GetNearestIncompleteSchedule(string deskId)
    {
        var schedule = await _repository.ReadNearestIncomplete(deskId);
        return schedule is null ? null : FlatScheduleDto.FromEntity(schedule);
    }
    
    
    [HttpGet("/api/[controller]/Data/{deskId}/{startDateTime:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ScheduleDataDto>> GetScheduleData(string deskId, DateTime startDateTime)
    {
        var schedule = await _repository.ReadAsync((deskId, startDateTime));
        if (schedule is null)
        {
            return NotFound();
        }
        
        var data = await _repository.GetScheduleData(deskId, startDateTime);
        data.Schedule = schedule;
        return ScheduleDataDto.FromEntity(data);
    }
    
    [HttpPost("/api/[controller]/Report/{deskId}/{startDateTime:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ScheduleReportDto>> GetScheduleReport(string deskId, DateTime startDateTime, ScheduleDto schedule)
    {
        if (startDateTime != schedule.StartDateTime || deskId != schedule.Desk.Id)
        {
            return BadRequest();
        }
    
        var report = await _repository.GetScheduleReport(schedule.ToEntity());
        return ScheduleReportDto.FromEntity(report);
    }

    [HttpGet("{deskId}/Current")]
    [Authorize]
    public async Task<ScheduleDto?> GetCurrentSchedule(string deskId)
    {
        var result = await _repository.ReadCurrentAsync(deskId);
        return result is null ? null : ScheduleDto.FromEntity(result);
    }
    
    [HttpGet("{deskId}/Next")]
    [Authorize]
    public async Task<ScheduleDto?> GetNextSchedule(string deskId)
    {
        var result = await _repository.ReadNextAsync(deskId);
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

    [HttpPatch("Assign/{deskId}/{startDateTime:datetime}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AssignEmployees(string deskId, DateTime startDateTime, ScheduleDto schedule)
    {
        if (schedule.StartDateTime != startDateTime || schedule.Desk.Id != deskId)
        {
            return BadRequest("schedule-key mismatch");
        }
        
        var assignedSchedule = schedule.ToEntity();
        await _repository.AssignEmployees(deskId, startDateTime, assignedSchedule);
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

    [HttpPost("create-schedule-image")]
    [Authorize]
    public async Task<IActionResult> CreateScheduleImageAsync(string deskId, DateTime scheduleStartDateTime)
    {
        var schedule = await _repository.ReadAsync((deskId, scheduleStartDateTime));
        if (schedule is null)
        {
            return NotFound("unable to find schedule in database.");
        }

        var url = await _imageService.Run(schedule);
        return Content(url);
    }
    
    [HttpPost("create-schedule-image/{employeeId:int}")]
    [Authorize]
    public async Task<IActionResult> CreateScheduleImageAsync(int employeeId, string deskId, DateTime scheduleStartDateTime)
    {
        var schedule = await _repository.ReadAsync((deskId, scheduleStartDateTime));
        if (schedule is null)
        {
            return NotFound("unable to find schedule in database.");
        }

        var employee = await _employeeRepository.ReadAsync(employeeId);
        if (employee is null)
        {
            return NotFound("unable to find employee in database.");
        }

        var url = await _imagePublisher.PublishScheduleImage(schedule, employee);
        return Content(url);
    }
}
