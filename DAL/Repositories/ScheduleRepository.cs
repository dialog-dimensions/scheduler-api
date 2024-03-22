using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Factories;
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


    public override async Task CreateAsync(Schedule entity)
    {
        Context.Shifts.AddRange(entity);
        await Context.SaveChangesAsync();
    }

    public override async Task<Schedule?> ReadAsync(object key)
    {
        var shifts = await Context.Shifts
            .Where(shift => shift.ScheduleKey == (DateTime)key)
            .ToListAsync();

        return shifts.Count == 0 ? null : _factory.FromShifts(shifts);
    }

    public override async Task<IEnumerable<Schedule>> ReadAllAsync()
    {
        var allShifts = await Context.Shifts.ToListAsync();

        return (allShifts.Count == 0 
            ? new List<Schedule>() 
            : allShifts.GroupBy(shift => shift.ScheduleKey).Select(_factory.FromShifts))!;
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

    public async Task<Schedule?> ReadLatestAsync()
    {
        var latestScheduleKey = await Context.Shifts
            .Select(shift => shift.ScheduleKey)
            .Distinct()
            .OrderByDescending(key => key)
            .FirstOrDefaultAsync();

        if (latestScheduleKey == default)
        {
            return null;
        }
        
        var latestShifts = await Context.Shifts
            .Where(shift => shift.ScheduleKey == latestScheduleKey)
            .ToListAsync();

        return _factory.FromShifts(latestShifts.ToList());
    }

    public async Task<Schedule?> ReadCurrentAsync()
    {
        var greatestScheduleKeySmallerThanNow = await Context.Shifts
            .Select(shift => shift.ScheduleKey)
            .Where(key => key < DateTime.Now)
            .MaxAsync();

        var shiftsOfSuspectKey = await Context.Shifts
            .Where(shift => shift.ScheduleKey == greatestScheduleKeySmallerThanNow)
            .Include(shift => shift.Employee)
            .OrderBy(shift => shift.StartDateTime)
            .ToListAsync();

        var suspectScheduleStart = shiftsOfSuspectKey[0].StartDateTime;
        var suspectScheduleEnd = shiftsOfSuspectKey[^1].EndDateTime;

        if (suspectScheduleStart < DateTime.Now && DateTime.Now < suspectScheduleEnd)
        {
            return _factory.FromShifts(shiftsOfSuspectKey);
        }

        return null;
    }
    
    public async Task<Schedule?> ReadNextAsync()
    {
        var scheduleKeysGreaterThanNow =
            await Context.Shifts.Select(shift => shift.ScheduleKey).Where(key => key > DateTime.Now).ToListAsync();

        if (scheduleKeysGreaterThanNow.Count == 0)
        {
            return null;
        }
        
        var smallestScheduleKeyGreaterThanNow = scheduleKeysGreaterThanNow.Min();

        var shifts = await Context.Shifts
            .Where(shift => shift.ScheduleKey == smallestScheduleKeyGreaterThanNow)
            .Include(shift => shift.Employee)
            .OrderBy(shift => shift.StartDateTime)
            .ToListAsync();

        return _factory.FromShifts(shifts);
    }

    public async Task<ScheduleData> GetScheduleData(DateTime scheduleKey)
    {
        return await _gatherer.GatherDataAsync(scheduleKey);
    }

    public async Task<ScheduleReport> GetScheduleReport(Schedule schedule)
    {
        var data = await _gatherer.GatherDataAsync(schedule.StartDateTime);
        data.Schedule = schedule;
        return _reportBuilder.BuildReport(data);
    }

    public async Task AssignEmployees(DateTime key, Schedule assignedSchedule)
    {
        var shiftsFromDatabase = await Context.Shifts
            .Where(shift => shift.ScheduleKey == key)
            .OrderBy(shift => shift.StartDateTime)
            .ToListAsync();
        
        for (var i = 0; i < assignedSchedule.Count; i++)
        {
            shiftsFromDatabase[i].Employee = await Context.Employees.FindAsync(assignedSchedule[i].Employee?.Id);
        }
        
        await Context.SaveChangesAsync();
    }

    public async Task<Schedule?> ReadNearestIncomplete()
    {
        var nearestIncompleteKey = await Context.Shifts
            .Where(shift => shift.EmployeeId == null || shift.EmployeeId == 0)
            .Select(shift => shift.ScheduleKey)
            .Distinct()
            .Where(key => key > DateTime.Now)
            .OrderByDescending(key => key)
            .FirstOrDefaultAsync();

        if (nearestIncompleteKey == default)
        {
            return null;
        }
        
        var latestShifts = await Context.Shifts
            .Where(shift => shift.ScheduleKey == nearestIncompleteKey)
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