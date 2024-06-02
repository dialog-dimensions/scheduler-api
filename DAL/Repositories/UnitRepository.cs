using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.DataStructures.Tree;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Entities.Workers.BaseClasses;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.DAL.Repositories;

public class UnitRepository : Repository<Unit>, IUnitRepository
{
    public UnitRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task<object> CreateAsync(Unit entity)
    {
        var entityEntry = Context.Units.Add(entity);
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }

    public override async Task<Unit?> ReadAsync(object key)
    {
        if (key is not string unitId)
        {
            throw new ArgumentException("key is not of expected type.");
        }
        return await Context.Units
            .Include(u => u.ParentUnit)
            .FirstOrDefaultAsync(u => u.Id.Equals(unitId));
    }

    public override async Task<IEnumerable<Unit>> ReadAllAsync()
    {
        return await Context.Units
            .Include(unit => unit.ParentUnit)
            .ToListAsync();
    }

    public override async Task DeleteAsync(object key)
    {
        var obj = await ReadAsync(key);
        if (obj is null)
        {
            throw new KeyNotFoundException("unit not found in database.");
        }

        await DeleteAsync(obj);
    }

    public override async Task DeleteAsync(Unit entity)
    {
        var desks = await Context.Desks.Where(d => d.UnitId == entity.Id).ToListAsync();
        var employees = await Context.Employees.Where(e => e.UnitId == entity.Id).ToListAsync();
        var deskAssignments = await Context.DeskAssignments
            .Where(da => 
                desks.Select(d => d.Id).Contains(da.DeskId) || 
                employees.Select(e => e.Id).Contains(da.EmployeeId)
                )
            .ToListAsync();

        Context.DeskAssignments.RemoveRange(deskAssignments);
        Context.Desks.RemoveRange(desks);
        Context.Employees.RemoveRange(employees);
        Context.Units.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<Unit?> ReadByNameAsync(string name)
    {
        return await Context.Units.FirstOrDefaultAsync(u => u.Name == name);
    }

    public async Task<Organization?> GetUnderlyingOrganizationAsync(string id)
    {
        var allUnits = (await ReadAllAsync()).ToList();
        var myUnit = allUnits.FirstOrDefault(unit => unit.Id == id);
        if (myUnit is null)
        {
            return default;
        }

        return GetSubOrganization(allUnits, myUnit, default);
    }

    public async Task<IEnumerable<Organization>> GetAllOrganizations()
    {
        var allUnits = await Context.Units
            .Include(u => u.ParentUnit)
            .ToListAsync();
        return allUnits
            .Where(u => u.ParentUnit == null)
            .Select(u => GetSubOrganization(allUnits, u, default));
    }

    public async Task<IEnumerable<Employee>> GetUnitEmployees(string id)
    {
        return await Context.Employees.Where(emp => emp.UnitId == id).ToListAsync();
    }

    private Organization GetSubOrganization(List<Unit> allUnits, Unit unit, TreeNode<Unit>? parentNode)
    {
        var myNode = new TreeNode<Unit>
        {
            Value = unit, 
            ParentNode = parentNode,
        };

        myNode.ChildNodes = allUnits
            .Where(u => u.ParentUnit?.Equals(unit) ?? false)
            .Select(u => GetSubOrganization(allUnits, u, myNode))
            .Select(org => org.Root);
        
        return new Organization { Root = myNode };
    }
}
