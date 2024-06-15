using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Services.ChatGptServices.Utils;
using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ShiftCommands;

public class AssignShiftEmployeeCommand : IGptCommand
{
    private readonly IQueryService _query;
    private readonly IShiftRepository _shiftRepository;
    
    public AssignShiftEmployeeCommand(IQueryService query, IShiftRepository shiftRepository)
    {
        _query = query;
        _shiftRepository = shiftRepository;
    }

    public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
    {
        // Find Shift
        var foundSingleShift = (await _query.Query<Shift>(parameters))
            .ToList()
            .ValidateSingleEntry(out var shiftQueryResponse);

        if (!foundSingleShift)
        {
            return shiftQueryResponse;
        }
        
        // Find Employee
        var foundSingleEmployee = (await _query.Query<Employee>(parameters))
            .ToList()
            .ValidateSingleEntry(out var employeeQueryResponse);

        if (!foundSingleEmployee)
        {
            return employeeQueryResponse;
        }

        // Assign Employee to Shift
        Shift shift;
        Employee employee;

        try
        {
            shift = (Shift)shiftQueryResponse.Content!;
            employee = (Employee)employeeQueryResponse.Content!;
        }
        catch (InvalidCastException ex)
        {
            return Problem(
                "an unhandled exception occured when casting entities back from the validator. " + ex.Message);
        }
        
        // Save Changes to Database
        try
        {
            shift.Employee = employee;
            await _shiftRepository.UpdateAsync(shift);
        }
        catch (Exception ex)
        {
            return Problem("an error occured when saving the entity to the database. please try again." +
                           ex.Message);
        }

        return Ok();
    }
}
