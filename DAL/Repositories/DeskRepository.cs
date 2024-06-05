using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.DAL.Repositories;

public class DeskRepository : Repository<Desk>, IDeskRepository
{
    public DeskRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task<object> CreateAsync(Desk entity)
    {
        if (entity.Unit is not null)
        {
            Context.Units.Attach(entity.Unit);
        }
        
        var entityEntry = Context.Desks.Add(entity);
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }

    public override async Task<Desk?> ReadAsync(object key)
    {
        if (key is not string deskId)
        {
            throw new ArgumentException("unexpected key type.");
        }

        return await Context.Desks
            .Include(desk => desk.Unit)
            .FirstOrDefaultAsync(desk => desk.Id == deskId);
    }

    public override async Task<IEnumerable<Desk>> ReadAllAsync()
    {
        return await Context.Desks
            .Include(desk => desk.Unit)
            .ToListAsync();
    }

    public override async Task DeleteAsync(object key)
    {
        var obj = await ReadAsync(key);
        if (obj is null)
        {
            throw new KeyNotFoundException();
        }

        await DeleteAsync(obj);
    }

    public override async Task DeleteAsync(Desk entity)
    {
        Context.Desks.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Desk>> ReadAllActiveUnit(Unit unit)
    {
        return await Context.Desks
            .Include(d => d.Unit)
            .Where(d => d.Unit.Equals(unit))
            .Where(d => d.Active)
            .ToListAsync();
    }

    public async Task<IEnumerable<Desk>> ReadAllUnit(Unit unit)
    {
        return await Context.Desks
            .Include(d => d.Unit)
            .Where(d => d.Unit.Equals(unit))
            .ToListAsync();
    }

    public async Task<IEnumerable<Desk>> ReadAllUnit(string unitId)
    {
        var unit = await Context.Units.FindAsync(unitId);
        if (unit is null)
        {
            return new List<Desk>();
        }

        return await ReadAllUnit(unit);
    }

    public async Task<IEnumerable<Employee>> GetDeskEmployees(Desk desk)
    {
        return await Context.DeskAssignments
            .Include(da => da.Employee)
            .ThenInclude(emp => emp.Unit)
            .Include(da => da.Desk)
            .ThenInclude(d => d.Unit)
            .Where(da => da.DeskId == desk.Id)
            .Select(da => da.Employee)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetDeskEmployees(string deskId)
    {
        var desk = await Context.Desks
            .Include(desk => desk.Unit)
            .FirstOrDefaultAsync(desk => desk.Id == deskId);
        
        if (desk is null)
        {
            return new List<Employee>();
        }

        return await GetDeskEmployees(desk);
    }

    public async Task<IEnumerable<Desk>> GetEmployeeDesks(Employee employee)
    {
        return await Context.DeskAssignments
            .Include(da => da.Desk)
            .ThenInclude(desk => desk.Unit)
            .Where(da => da.EmployeeId == employee.Id)
            .Select(da => da.Desk)
            .ToListAsync();
    }

    public async Task<IEnumerable<Desk>> GetEmployeeDesks(int employeeId)
    {
        var employee = await Context.Employees
            .Include(emp => emp.Unit)
            .FirstOrDefaultAsync(emp => emp.Id == employeeId);
        
        if (employee is null)
        {
            return new List<Desk>();
        }

        return await GetEmployeeDesks(employee);
    }

    public async Task AddDeskAssignment(Desk desk, Employee employee)
    {
        await AddDeskAssignment(new DeskAssignment { Desk = desk, Employee = employee });
    }

    private async Task AddDeskAssignment(DeskAssignment assignment)
    {
        Context.DeskAssignments.Add(assignment);
        await Context.SaveChangesAsync();
    }

    public async Task RemoveDeskAssignment(Desk desk, Employee employee)
    {
        var assignment = await Context.DeskAssignments.FindAsync(desk.Id, employee.Id);
        if (assignment is null)
        {
            throw new KeyNotFoundException("assignment not found in database.");
        }
        
        await RemoveDeskAssignment(assignment);
    }

    private async Task RemoveDeskAssignment(DeskAssignment assignment)
    {
        Context.DeskAssignments.Remove(assignment);
        await Context.SaveChangesAsync();
    }

    public async Task RemoveDeskAssignments(Desk desk)
    {
        var assignments = await Context.DeskAssignments
            .Where(da => da.DeskId == desk.Id)
            .ToListAsync();
        await RemoveRange(assignments);
    }

    public async Task RemoveEmployeeAssignments(Employee employee)
    {
        var assignments = await Context.DeskAssignments
            .Where(da => da.EmployeeId == employee.Id)
            .ToListAsync();
        await RemoveRange(assignments);
    }

    private async Task RemoveRange(IEnumerable<DeskAssignment> assignments)
    {
        Context.DeskAssignments.RemoveRange(assignments);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateProcessParametersAsync(string deskId, string catchRangeString, string fileWindowDurationString,
        string headsUpDurationString)
    {
        var desk = await ReadAsync(deskId);
        if (desk is null)
        {
            return;
        }
        
        var newProcessParameters = new ProcessParameters
        {
            CatchRange = TimeSpan.Parse(catchRangeString),
            FileWindowDuration = TimeSpan.Parse(fileWindowDurationString),
            HeadsUpDuration = TimeSpan.Parse(headsUpDurationString)
        };

        await UpdateProcessParametersAsync(desk, newProcessParameters);
    }
    
    private async Task UpdateProcessParametersAsync(Desk desk, ProcessParameters processParameters)
    {
        desk.ProcessParameters = processParameters;
        Context.Desks.Update(desk);
        await Context.SaveChangesAsync();
    }
    
    
    
}
