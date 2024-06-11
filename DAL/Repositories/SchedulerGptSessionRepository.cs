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

    public override async Task<IEnumerable<GathererGptSession>> Query(Dictionary<string, object> parameters, string prefixDiscriminator = "")
    {
        var hasThreadId = parameters.TryGetValue("ThreadId", out var threadIdValue);
        if (hasThreadId)
        {
            var threadId = Convert.ToString(threadIdValue);
            var session = await ReadAsync(threadId!);
            if (session is null)
            {
                return new GathererGptSession[] { };
            }

            return new[] { session };
        }

        var matches = Context.SchedulerGptSessions.AsQueryable();

        var hasDeskId = parameters.TryGetValue("DeskId", out var deskIdValue);
        if (hasDeskId)
        {
            var deskId = Convert.ToString(deskIdValue);
            matches = matches.Where(session => session.DeskId == deskId);
        }

        var hasScheduleStartDateTime =
            parameters.TryGetValue("ScheduleStartDateTime", out var scheduleStartDateTimeValue);
        if (hasScheduleStartDateTime)
        {
            var scheduleStartDateTime = Convert.ToDateTime(scheduleStartDateTimeValue);
            matches = matches.Where(s => s.ScheduleStartDateTime == scheduleStartDateTime);
        }

        var hasSessionConversationState =
            parameters.TryGetValue("SessionConversationState", out var sessionConversationStateValue);
        if (hasSessionConversationState)
        {
            var sessionConversationStateString = Convert.ToString(sessionConversationStateValue);
            var sessionConversationState =
                (ShabtzanGptConversationState)Enum.Parse(typeof(ShabtzanGptConversationState),
                    sessionConversationStateString!);
            matches = matches.Where(s => s.ConversationState == sessionConversationState);
        }

        return await matches.ToListAsync();
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
