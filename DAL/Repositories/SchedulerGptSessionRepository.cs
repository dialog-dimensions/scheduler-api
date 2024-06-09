using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT;
using SchedulerApi.Models.ChatGPT.Sessions;
using SchedulerApi.Services.ChatGptServices;

namespace SchedulerApi.DAL.Repositories;

public class SchedulerGptSessionRepository : Repository<GathererGptSession>, ISchedulerGptSessionRepository
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IChatGptClient _chatGptClient;
    
    public SchedulerGptSessionRepository(ApiDbContext context, IScheduleRepository scheduleRepository, IChatGptClient chatGptClient) : base(context)
    {
        _scheduleRepository = scheduleRepository;
        _chatGptClient = chatGptClient;
    }

    public override async Task<object> CreateAsync(GathererGptSession entity)
    {
        var entityEntry = Context.SchedulerGptSessions.Add(entity);
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }
    
    public override async Task<GathererGptSession?> ReadAsync(object key)
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
        result.Messages = await _chatGptClient.ThreadListMessagesAsync(result.ThreadId);
        
        // Return Session
        return result;
    }

    public override async Task<IEnumerable<GathererGptSession>> ReadAllAsync()
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

    public override async Task DeleteAsync(GathererGptSession entity)
    {
        Context.SchedulerGptSessions.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<GathererGptSession?> FindActiveByEmployeeIdAsync(int employeeId)
    {
        return await Context.SchedulerGptSessions.FirstOrDefaultAsync(
            session => session.EmployeeId == employeeId &&
                       session.ConversationState != ShabtzanGptConversationState.Ended &&
                       session.ConversationState != ShabtzanGptConversationState.Faulted
                       );
    }

    public override async Task UpdateAsync(GathererGptSession entity)
    {
        Context.SchedulerGptSessions.Update(entity);
        await Context.SaveChangesAsync();
    }
}
