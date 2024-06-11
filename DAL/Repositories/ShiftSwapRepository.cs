using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.DAL.Repositories;

public class ShiftSwapRepository : Repository<ShiftSwap>, IShiftSwapRepository
{
    public ShiftSwapRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task<object> CreateAsync(ShiftSwap entity)
    {
        var entityEntry = Context.Swaps.Add(entity);
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }

    public override async Task<ShiftSwap?> ReadAsync(object key)
    {
        var result = await Context.Swaps.FirstOrDefaultAsync(swap => swap.Key.Equals(key));
        return result;
    }

    public override async Task<IEnumerable<ShiftSwap>> ReadAllAsync()
    {
        var result = await Context.Swaps.ToListAsync();
        return result;
    }

    public override async Task DeleteAsync(object key)
    {
        var entity = await Context.Swaps.FirstOrDefaultAsync(swap => swap.Key.Equals(key));
        await DeleteAsync(entity ?? throw new KeyNotFoundException("Swap not found in database."));
    }

    public override async Task DeleteAsync(ShiftSwap entity)
    {
        if (!Context.Swaps.Contains(entity))
        {
            throw new KeyNotFoundException("Swap not found in database.");
        }

        Context.Swaps.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public override Task<IEnumerable<ShiftSwap>> Query(Dictionary<string, object> parameters, string prefixDiscriminator = "")
    {
        throw new NotImplementedException();
    }
}