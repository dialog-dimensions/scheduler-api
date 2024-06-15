using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Sessions;
using SchedulerApi.Models.ChatGPT.Sessions.BaseClasses;

namespace SchedulerApi.DAL.Repositories;

public class ManagerSupportGptSessionRepository : Repository<ManagerSupportGptSession>, IManagerSupportGptSessionRepository
{
    public ManagerSupportGptSessionRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task<object> CreateAsync(ManagerSupportGptSession entity)
    {
        var entryEntity = Context.ManagerSupportGptSessions.Add(entity);
        await Context.SaveChangesAsync();
        return entryEntity.Entity.Key;
    }

    public override async Task<ManagerSupportGptSession?> ReadAsync(object key)
    {
        if (key is not string threadId)
        {
            return null;
        }
        
        return await Context.ManagerSupportGptSessions.FindAsync(threadId);
    }

    public override async Task<IEnumerable<ManagerSupportGptSession>> ReadAllAsync()
    {
        return await Context.ManagerSupportGptSessions.ToListAsync();
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

    public override async Task DeleteAsync(ManagerSupportGptSession entity)
    {
        Context.ManagerSupportGptSessions.Remove(entity);
        await Context.SaveChangesAsync();
    }


            return new[] { process };
        }
        
        return new ManagerSupportGptSession[] { };  
    }

    public override async Task UpdateAsync(ManagerSupportGptSession entity)
    {
        Context.ManagerSupportGptSessions.Update(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<ManagerSupportGptSession?> FindByManagerIdAsync(int managerId)
    {
        return await Context.ManagerSupportGptSessions.FirstOrDefaultAsync(session =>
            session.EmployeeId == managerId && 
            (
                session.State == GptSessionState.Open || 
                session.State == GptSessionState.Created
                )
            );
    }
}
