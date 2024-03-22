using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine;

public class DataGatherer : IDataGatherer
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IShiftExceptionRepository _exceptionRepository;

    public DataGatherer(IEmployeeRepository employeeRepository, IScheduleRepository scheduleRepository,
        IShiftExceptionRepository exceptionRepository)
    {
        _employeeRepository = employeeRepository;
        _scheduleRepository = scheduleRepository;
        _exceptionRepository = exceptionRepository;
    }
    
    public async Task<ScheduleData> GatherDataAsync(DateTime scheduleKey)
    {
        var schedule = await _scheduleRepository.ReadAsync(scheduleKey);
        if (schedule is null)
        {
            throw new KeyNotFoundException("Schedule not found in database.");
        }

        var activeEmployees = (await _employeeRepository.ReadAllActiveAsync()).ToList();
        if (activeEmployees.Count == 0)
        {
            throw new ApplicationException("No active employees are present in the database.");
        }

        var exceptions = await _exceptionRepository.GetScheduleExceptions(scheduleKey);

        return new ScheduleData { Schedule = schedule, Employees = activeEmployees, Exceptions = exceptions };
    }
}