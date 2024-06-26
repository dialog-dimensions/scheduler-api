﻿using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Entities.Workers.BaseClasses;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.DAL.Repositories.Interfaces;

public interface IUnitRepository : IRepository<Unit>
{
    Task<IEnumerable<Unit>> FindByNameAsync(string name);
    Task<Organization?> GetUnderlyingOrganizationAsync(string id);
    Task<IEnumerable<Employee>> GetUnitEmployees(string id);
    Task<IEnumerable<Worker>> GetUnitManagers(string id);
}
