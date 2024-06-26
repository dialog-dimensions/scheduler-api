﻿using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> ReadAllActiveAsync();

    Task<IEnumerable<Employee>> ReadAllActiveAsync(string deskId);
    
    Task<IEnumerable<Employee>> ReadAllAssignedAsync();

    Task IncrementRegularBalance(int employeeId, double increment);
    Task IncrementDifficultBalance(int employeeId, double increment);
    Task<IEnumerable<Employee>> GetUnitManagers(string unitId);

    Task<IEnumerable<Employee>> FindByNameAndUnitId(string name, string unitId);
}
