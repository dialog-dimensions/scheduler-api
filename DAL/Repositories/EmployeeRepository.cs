using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.DAL.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository 
{
    public EmployeeRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task<object> CreateAsync(Employee entity)
    {
        if (entity.Unit is { } unit)
        {
            Context.Units.Attach(unit);
        }
        var entityEntry = Context.Employees.Add(entity);
        await Context.SaveChangesAsync();
        return entityEntry.Entity.Key;
    }

    public override async Task<Employee?> ReadAsync(object key)
    {
        if (key is not int id)
        {
            throw new ArgumentException("unexpected key type.");
        }
        
        var result = await Context.Employees
            .Include(emp => emp.Unit)
            .FirstOrDefaultAsync(emp => emp.Id == id);
        return result;
    }

    public override async Task<IEnumerable<Employee>> ReadAllAsync() => 
        await Context.Employees
            .Include(emp => emp.Unit)
            .ToListAsync();

    public override async Task DeleteAsync(object key)
    {
        var entity = await Context.Employees.FindAsync(key);
        if (entity is null)
        {
            return;
        }

        Context.Employees.Remove(entity);
        await Context.SaveChangesAsync();
    }

    public override async Task DeleteAsync(Employee entity)
    {
        if (!Context.Employees.Contains(entity))
        {
            throw new KeyNotFoundException("Employee not found in database.");
        }

        Context.Employees.Remove(entity);
        await Context.SaveChangesAsync();
    }


        var matches = Context.Employees.AsQueryable();
        
        var hasEmployeeName = parameters.TryGetValue("EmployeeName", out var employeeNameValue);
        if (hasEmployeeName)
        {
            var employeeName = Convert.ToString(employeeNameValue);
            matches = matches.Where(emp => emp.Name == employeeName);
        }

        var hasEmployeeBalance = parameters.TryGetValue("EmployeeBalance", out var employeeBalanceValue);
        if (hasEmployeeBalance)
        {
            var employeeBalance = Convert.ToDouble(employeeBalanceValue);
            matches = matches.Where(emp => Math.Abs(emp.Balance - employeeBalance) < 0.004);
        }

        var hasEmployeeDifficultBalance =
            parameters.TryGetValue("EmployeeDifficultBalance", out var employeeDifficultBalanceValue);
        if (hasEmployeeDifficultBalance)
        {
            var employeeDifficultBalance = Convert.ToDouble(employeeDifficultBalanceValue);
            matches = matches.Where(emp => Math.Abs(emp.DifficultBalance - employeeDifficultBalance) < 0.004);
        }

        var hasEmployeeActive = parameters.TryGetValue("EmployeeActive", out var employeeActiveValue);
        if (hasEmployeeActive)
        {
            var employeeActive = Convert.ToBoolean(employeeActiveValue);
            matches = matches.Where(emp => emp.Active == employeeActive);
        }

        var hasEmployeeRole = parameters.TryGetValue("EmployeeRole", out var employeeRoleValue);
        if (hasEmployeeRole)
        {
            var employeeRole = Convert.ToString(employeeRoleValue);
            matches = matches.Where(emp => emp.Role == employeeRole);
        }

        var hasUnitId = parameters.TryGetValue("UnitId", out var unitIdValue);
        if (hasUnitId)
        {
            var unitId = Convert.ToString(unitIdValue);
            matches = matches.Where(emp => emp.UnitId == unitId);
        }

        var hasEmployeeGender = parameters.TryGetValue("EmployeeGender", out var employeeGenderValue);
        if (hasEmployeeGender)
        {
            var employeeGenderString = Convert.ToString(employeeGenderValue);
            var employeeGender = (Gender)Enum.Parse(typeof(Gender), employeeGenderString!);
            matches = matches.Where(emp => emp.Gender == employeeGender);
        }

        return await matches.ToListAsync();
    }

    public async Task<IEnumerable<Employee>> ReadAllActiveAsync()
    {
        var result = await Context.Employees
            .Include(emp => emp.Unit)
            .Where(emp => emp.Active).ToListAsync();
        return result;
    }

    public async Task<IEnumerable<Employee>> ReadAllActiveAsync(string deskId)
    {
        return await Context.DeskAssignments
            .Where(da => da.DeskId == deskId)
            .Include(da => da.Employee)
            .ThenInclude(emp => emp.Unit)
            .Select(da => da.Employee)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> ReadAllAssignedAsync()
    {
        // TODO: implement in O(1).
        
        var result = new HashSet<Employee>();
        var allShifts = await Context.Shifts
            .Include(shift => shift.Employee)
            .ThenInclude(emp => emp.Unit)
            .Include(shift => shift.Desk)
            .ThenInclude(desk => desk.Unit)
            .ToListAsync();
        foreach (var shift in allShifts)
        {
            if (shift.Employee is null) continue;
            result.Add(shift.Employee);
        }

        return result;
    }

    public async Task IncrementRegularBalance(int employeeId, double increment)
    {
        var employee = await Context.Employees.FindAsync(employeeId);
        if (employee is null)
        {
            return;
        }

        employee.Balance += increment;
        Context.Employees.Update(employee);
        await Context.SaveChangesAsync();
    }

    public async Task IncrementDifficultBalance(int employeeId, double increment)
    {
        var employee = await Context.Employees.FindAsync(employeeId);
        if (employee is null)
        {
            return;
        }

        employee.DifficultBalance += increment;
        Context.Employees.Update(employee);
        await Context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Employee>> GetUnitManagers(string unitId) => 
        await Context.Employees
            .Where(e => e.Role == "Manager")
            .Where(e => e.UnitId == unitId)
            .ToListAsync();

    public async Task<IEnumerable<Employee>> FindByNameAndUnitId(string name, string unitId)
    {
        return await Context.Employees.Where(emp => emp.Name == name && emp.UnitId == unitId).ToListAsync();
    }

    public override async Task UpdateAsync(Employee employee)
    {
        Context.Entry(employee).State = EntityState.Modified;

        try
        {
            await Context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!Context.Employees.Any(emp => emp.Id == employee.Id))
            {
                throw new KeyNotFoundException("Employee not found in database.");
            }

            throw;
        }
    }
    
    
}