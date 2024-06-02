using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT;
using SchedulerApi.Services.ChatGptClient.Interfaces;

namespace SchedulerApi.DAL.Repositories;

public class SchedulerGptSessionRepository : Repository<SchedulerGptSession>, ISchedulerGptSessionRepository
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAssistantServices _assistantServices;
    
    public SchedulerGptSessionRepository(ApiDbContext context, IScheduleRepository scheduleRepository, IAssistantServices assistantServices) : base(context)
    {
        _scheduleRepository = scheduleRepository;
        _assistantServices = assistantServices;
    }

    public override async Task<object> CreateAsync(SchedulerGptSession entity)
    {
        var entityEntry = Context.SchedulerGptSessions.Add(entity);
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }
    
    public override async Task<SchedulerGptSession?> ReadAsync(object key)
    {
        if (key is not string threadId)
        {
            return null;
        }

        // Read Session Record and Include Employee Navigation Property
        var result = await Context.SchedulerGptSessions
            .Include(session => session.Employee)
            .FirstOrDefaultAsync(session => session.ThreadId == threadId);

        if (result is null)
        {
            return null;
        }
        
        // Get Session Schedule Record
        var schedule = await _scheduleRepository.ReadAsync((result.DeskId, result.ScheduleStartDateTime));
        result.Schedule = schedule;
        
        // Get Thread Messages
        result.Messages = await _assistantServices.ThreadListMessagesAsync(result.ThreadId);
        
        // Return Session
        return result;
    }

    public override async Task<IEnumerable<SchedulerGptSession>> ReadAllAsync()
    {
        return await Context.SchedulerGptSessions.ToListAsync();
    }

    public override async Task DeleteAsync(object key)
    {
        var entity = await ReadAsync(key);
        if (entity is null)
        {
            return;
        }

        await DeleteAsync(entity);
    }

    public override async Task DeleteAsync(SchedulerGptSession entity)
    {
        Context.SchedulerGptSessions.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<SchedulerGptSession?> FindActiveByEmployeeIdAsync(int employeeId)
    {
        return await Context.SchedulerGptSessions.FirstOrDefaultAsync(
            session => session.EmployeeId == employeeId &&
                       session.ConversationState != ShabtzanGptConversationState.Ended &&
                       session.ConversationState != ShabtzanGptConversationState.Faulted
                       );
    }

    public override async Task UpdateAsync(SchedulerGptSession entity)
    {
        Context.SchedulerGptSessions.Update(entity);
        await Context.SaveChangesAsync();
    }
}
