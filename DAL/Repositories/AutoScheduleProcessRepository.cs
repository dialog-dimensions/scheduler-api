using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Services.Workflows.Processes.Classes;

namespace SchedulerApi.DAL.Repositories;

public class AutoScheduleProcessRepository : Repository<AutoScheduleProcess>, IAutoScheduleProcessRepository
{


    public AutoScheduleProcessRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task<object> CreateAsync(AutoScheduleProcess entity)
    {
        var entityEntry = Context.AutoScheduleProcesses.Add(entity);
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }

    public override async Task<AutoScheduleProcess?> ReadAsync(object key)
    {
        if (key is not int id) throw new ArgumentException("key is not of expected type.");
        return await Context.AutoScheduleProcesses
            .Include(p => p.Desk)
            .ThenInclude(desk => desk.Unit)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<AutoScheduleProcess>> ReadAllAsync()
    {
        return await Context.AutoScheduleProcesses
            .Include(p => p.Desk)
            .ThenInclude(desk => desk.Unit)
            .ToListAsync();
    }

    public override async Task DeleteAsync(object key)
    {
        if (key is not int id)
        {
            throw new ArgumentException("key is not of expected type.");
        }
        
        var obj = await ReadAsync(id);
        if (obj is null)
        {
            return;
        }

        await DeleteAsync(obj);
    }

    public override async Task DeleteAsync(AutoScheduleProcess entity)
    {
        Context.AutoScheduleProcesses.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public override async Task<IEnumerable<AutoScheduleProcess>> Query(Dictionary<string, object> parameters, string prefixDiscriminator = "")
    {
        var hasProcessId = parameters.TryGetValue($"{prefixDiscriminator}ProcessId", out var processIdValue);
        if (hasProcessId)
        {
            var processId = Convert.ToInt32(processIdValue);
            var process = await ReadAsync(processId);
            if (process is null)
            {
                return new AutoScheduleProcess[] {};
            }
            
            return new[] { process };
        }

        var matches = Context.AutoScheduleProcesses.AsQueryable();
        
        var hasProcessStartDateTime = parameters.TryGetValue($"{prefixDiscriminator}ProcessStartDateTime", out var processStartDateTimeValue);
        if (hasProcessStartDateTime)
        {
            var processStartDateTime = Convert.ToDateTime(processStartDateTimeValue);
            matches = matches.Where(p => p.ProcessStart == processStartDateTime);
        }

        var hasProcessFileWindowEndDateTime =
            parameters.TryGetValue($"{prefixDiscriminator}ProcessFileWindowEndDateTime", out var processFileWindowEndDateTimeValue);
        if (hasProcessFileWindowEndDateTime)
        {
            var processFileWindowEndDateTime = Convert.ToDateTime(processFileWindowEndDateTimeValue);
            matches = matches.Where(p => p.FileWindowEnd == processFileWindowEndDateTime);
        }

        var hasProcessPublishDateTime =
            parameters.TryGetValue($"{prefixDiscriminator}ProcessPublishDateTime", out var processPublishDateTimeValue);
        if (hasProcessPublishDateTime)
        {
            var processPublishDateTime = Convert.ToDateTime(processPublishDateTimeValue);
            matches = matches.Where(p => p.PublishDateTime == processPublishDateTime);
        }

        var hasScheduleStartDateTime =
            parameters.TryGetValue($"{prefixDiscriminator}ScheduleStartDateTime", out var scheduleStartDateTimeValue);
        if (hasScheduleStartDateTime)
        {
            var scheduleStartDateTime = Convert.ToDateTime(scheduleStartDateTimeValue);
            matches = matches.Where(p => p.ScheduleStart == scheduleStartDateTime);
        }

        var hasScheduleEndDateTime = parameters.TryGetValue($"{prefixDiscriminator}ScheduleEndDateTime", out var scheduleEndDateTimeValue);
        if (hasScheduleEndDateTime)
        {
            var scheduleEndDateTime = Convert.ToDateTime(scheduleEndDateTimeValue);
            matches = matches.Where(p => p.ScheduleEnd == scheduleEndDateTime);
        }

        var hasShiftDuration = parameters.TryGetValue($"{prefixDiscriminator}ShiftDuration", out var shiftDurationValue);
        if (hasShiftDuration)
        {
            var shiftDuration = Convert.ToInt32(shiftDurationValue);
            matches = matches.Where(p => p.ScheduleShiftDuration == shiftDuration);
        }

        var hasDeskId = parameters.TryGetValue($"{prefixDiscriminator}DeskId", out var deskIdValue);
        if (hasDeskId)
        {
            var deskId = Convert.ToString(deskIdValue);
            matches = matches.Where(p => p.DeskId == deskId);
        }

        return await matches.ToListAsync();
    }

    public async Task<AutoScheduleProcess?> ReadRunningAsync(string deskId, DateTime scheduleStart)
    {
        return await Context.AutoScheduleProcesses
            .Where(p => p.Status == TaskStatus.Running)
            .Where(p => p.DeskId == deskId)
            .Include(p => p.Desk)
            .ThenInclude(desk => desk.Unit)
            .FirstOrDefaultAsync(p => p.ScheduleStart == scheduleStart);
    }

    public override async Task UpdateAsync(AutoScheduleProcess entity)
    {
        Context.AutoScheduleProcesses.Update(entity);
        await Context.SaveChangesAsync();
    }
}
