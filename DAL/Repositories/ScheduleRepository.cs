using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Factories;
using SchedulerApi.Models.Organization;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.DAL.Repositories;

public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
{
    private readonly IScheduleFactory _factory;
    private readonly IDataGatherer _gatherer;
    private readonly IScheduleReportBuilder _reportBuilder;
    
    public ScheduleRepository(ApiDbContext context, IScheduleFactory factory, IDataGatherer gatherer, IScheduleReportBuilder reportBuilder) : base(context)
    {
        _factory = factory;
        _gatherer = gatherer;
        _reportBuilder = reportBuilder;
    }


    public override async Task<object> CreateAsync(Schedule entity)
    { 
        Context.Shifts.AddRange(entity);
        await Context.SaveChangesAsync();
        return entity.Key;
    }

    public override async Task<Schedule?> ReadAsync(object key)
    {
        if (key is not (string deskId, DateTime startDateTime))
        {
            throw new ArgumentException("schedule key is a composite key of deskId and startDateTime");
        }
        
        var shifts = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(shift => shift.ScheduleStartDateTime == startDateTime)
            .Where(shift => shift.Desk.Id == deskId)
            .ToListAsync();

        return shifts.Count == 0 ? null : _factory.FromShifts(shifts);
    }

    public override async Task<IEnumerable<Schedule>> ReadAllAsync()
    {
        var allShifts = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .ToListAsync();

        return (allShifts.Count == 0 
            ? new List<Schedule>() 
            : allShifts.GroupBy(shift => (shift.ScheduleStartDateTime, shift.Desk.Id)).Select(_factory.FromShifts))!;
    }

    // public async Task<IEnumerable<Schedule>> ReadAllFutureAsync()
    // {
    //     var allFutureSchedulesShifts = await Context.Shifts
    //         .Where(shift => shift.ScheduleKey > DateTime.Now)
    //         .ToListAsync();
    //
    //     return (allFutureSchedulesShifts.Count == 0
    //         ? new List<Schedule>()
    //         : allFutureSchedulesShifts.GroupBy(s => s.ScheduleKey).Select(_factory.FromShifts))!;
    // }

    public async Task<Schedule?> ReadLatestAsync(string deskId)
    {
        var latestScheduleStartDateTime = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(shift => shift.Desk.Id == deskId)
            .Select(shift => shift.ScheduleStartDateTime)
            .Distinct()
            .OrderByDescending(key => key)
            .FirstOrDefaultAsync();

        if (latestScheduleStartDateTime == default)
        {
            return null;
        }
        
        var latestShifts = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(shift => shift.Desk.Id == deskId)
            .Where(shift => shift.ScheduleStartDateTime == latestScheduleStartDateTime)
            .ToListAsync();

        return _factory.FromShifts(latestShifts.ToList());
    }

    public async Task<Dictionary<Desk, Schedule?>> ReadAllActiveLatest()
    {
        var desks = await Context.Desks
            .Include(desk => desk.Unit)
            .Where(d => d.Active)
            .ToListAsync();

        var result = new Dictionary<Desk, Schedule?>();
        foreach (var desk in desks)
        {
            result[desk] = await ReadLatestAsync(desk.Id);
        }

        return result;
    }

    public async Task<Schedule?> ReadCurrentAsync(string deskId)
    {
        var greatestScheduleStartSmallerThanNow = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(shift => shift.Desk.Id == deskId)
            .Where(shift => shift.ScheduleStartDateTime < DateTime.Now)
            .Select(shift => shift.ScheduleStartDateTime)
            .MaxAsync();

        var shiftsOfSuspectStart = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Where(shift => shift.Desk.Id == deskId)
            .Where(shift => shift.ScheduleStartDateTime == greatestScheduleStartSmallerThanNow)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .OrderBy(shift => shift.StartDateTime)
            .ToListAsync();

        var suspectScheduleStart = shiftsOfSuspectStart[0].StartDateTime;
        var suspectScheduleEnd = shiftsOfSuspectStart[^1].EndDateTime;

        if (suspectScheduleStart < DateTime.Now && DateTime.Now < suspectScheduleEnd)
        {
            return _factory.FromShifts(shiftsOfSuspectStart);
        }

        return null;
    }
    
    public async Task<Schedule?> ReadNextAsync(string deskId)
    {
        var scheduleStartDateTimesGreaterThanNow = 
            await Context.Shifts
                .Include(shift => shift.Desk)
                .ThenInclude(desk => desk.Unit)
                .Include(shift => shift.Employee)
                .ThenInclude(emp => emp.Unit)
                .Where(shift => shift.Desk.Id == deskId)
                .Where(shift => shift.ScheduleStartDateTime > DateTime.Now)
                .Select(shift => shift.ScheduleStartDateTime)
                .ToListAsync();

        if (scheduleStartDateTimesGreaterThanNow.Count == 0)
        {
            return null;
        }
        
        var smallestScheduleStartGreaterThanNow = scheduleStartDateTimesGreaterThanNow.Min();

        var shifts = await Context.Shifts
            .Where(shift => shift.ScheduleStartDateTime == smallestScheduleStartGreaterThanNow)
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .OrderBy(shift => shift.StartDateTime)
            .ToListAsync();

        // TODO: Employee unit not included, but employee is nullable, need to resolve.
        
        return _factory.FromShifts(shifts);
    }

    public async Task<ScheduleData> GetScheduleData(string deskId, DateTime scheduleStartDateTime)
    {
        return await _gatherer.GatherDataAsync(deskId, scheduleStartDateTime);
    }

    public async Task<ScheduleReport> GetScheduleReport(Schedule schedule)
    {
        var data = await _gatherer.GatherDataAsync(schedule.DeskId, schedule.StartDateTime);
        data.Schedule = schedule;
        return _reportBuilder.BuildReport(data);
    }

    public async Task AssignEmployees(string deskId, DateTime scheduleStart, Schedule assignedSchedule)
    {
        var shiftsFromDatabase = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(shift => shift.Desk.Id == deskId)
            .Where(shift => shift.ScheduleStartDateTime == scheduleStart)
            .OrderBy(shift => shift.StartDateTime)
            .ToListAsync();
        
        for (var i = 0; i < assignedSchedule.Count; i++)
        {
            shiftsFromDatabase[i].Employee = await Context.Employees.FindAsync(assignedSchedule[i].Employee?.Id);
        }
        
        await Context.SaveChangesAsync();
    }

    public async Task<Schedule?> ReadNearestIncomplete(string deskId)
    {
        var nearestIncompleteStartDateTime = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(shift => shift.Desk.Id == deskId)
            .Where(shift => shift.EmployeeId == null || shift.EmployeeId == 0)
            .Select(shift => shift.ScheduleStartDateTime)
            .Distinct()
            .Where(key => key > DateTime.Now)
            .OrderByDescending(key => key)
            .FirstOrDefaultAsync();

        if (nearestIncompleteStartDateTime == default)
        {
            return null;
        }
        
        var latestShifts = await Context.Shifts
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Where(shift => shift.Desk.Id == deskId)
            .Where(shift => shift.ScheduleStartDateTime == nearestIncompleteStartDateTime)
            .ToListAsync();

        return _factory.FromShifts(latestShifts.ToList());
    }

    // public async Task<IEnumerable<DateTime>> UpdateAsync(Schedule schedule)
    // {
    //     List<DateTime> faultyShifts = [];
    //     foreach (var shift in schedule)
    //     {
    //         var shift = schedule.FirstOrDefault(s => s.StartDateTime.Equals(shiftDto.StartDateTime));
    //         if (shift is null)
    //         {
    //             faultyShifts.Add(shiftDto.StartDateTime);
    //             continue;
    //         }
    //
    //         shift.EmployeeId = shiftDto.EmployeeId;
    //     }
    //
    //     if (faultyShifts.Count == 0)
    //     {
    //         await Context.SaveChangesAsync();
    //     }
    //     
    //     return faultyShifts;
    // }

    public override Task DeleteAsync(object key)
    {
        throw new NotImplementedException();
    }

    public override Task DeleteAsync(Schedule entity)
    {
        throw new NotImplementedException();
    }
}