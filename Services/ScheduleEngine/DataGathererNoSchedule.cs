﻿using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine;

public class DataGathererNoSchedule : IDataGatherer
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IShiftExceptionRepository _exceptionRepository;

    public DataGathererNoSchedule(IEmployeeRepository employeeRepository, IShiftExceptionRepository exceptionRepository)
    {
        _employeeRepository = employeeRepository;
        _exceptionRepository = exceptionRepository;
    }
    
    public async Task<ScheduleData> GatherDataAsync(string deskId, DateTime scheduleStartDateTime)
    {
        var employees = await _employeeRepository.ReadAllActiveAsync(deskId);
        var exceptions = await _exceptionRepository.GetScheduleExceptions(deskId, scheduleStartDateTime);
        return new ScheduleData 
        { 
            Employees = employees, 
            Exceptions = exceptions 
        };
    }
}