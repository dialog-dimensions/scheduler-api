﻿using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.BaseClasses;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.DAL.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository 
{
    public EmployeeRepository(ApiDbContext context) : base(context)
    {
    }

    public override async Task CreateAsync(Employee entity)
    {
        Context.Employees.Add(entity);
        await Context.SaveChangesAsync();
    }

    public override async Task<Employee?> ReadAsync(object key)
    {
        var result = await Context.Employees.FindAsync(key);
        return result;
    }

    public override async Task<IEnumerable<Employee>> ReadAllAsync()
    {
        var result = await Context.Employees.ToListAsync();
        return result;
    }

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

    public async Task<IEnumerable<Employee>> ReadAllActiveAsync()
    {
        var result = await Context.Employees.Where(emp => emp.Active).ToListAsync();
        return result;
    }

    public async Task<IEnumerable<Employee>> ReadAllAssignedAsync()
    {
        // TODO: implement in O(1).
        
        var result = new HashSet<Employee>();
        var allShifts = await Context.Shifts.Include(shift => shift.Employee).ToListAsync();
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