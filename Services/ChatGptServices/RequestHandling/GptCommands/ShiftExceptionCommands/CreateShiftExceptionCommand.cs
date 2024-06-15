using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;
using SchedulerApi.Services.ChatGptServices.Utils;
using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;
using static SchedulerApi.Models.ChatGPT.Responses.EntityGptResponse;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ShiftExceptionCommands;

public class CreateShiftExceptionCommand : CreateCommand<ShiftException>
{
    private readonly IQueryService _queryService;
    
    public CreateShiftExceptionCommand(IShiftExceptionRepository entityRepository, IQueryService queryService) : base(entityRepository)
    {
        _queryService = queryService;
    }

    public override async Task<IGptResponse> ValidateEntityParameters(Dictionary<string, object> parameters)
    {
        // Find Desk
        var foundSingleDesk = (await _queryService.Query<Desk>(parameters))
            .ToList()
            .ValidateSingleEntry(out var deskQueryResponse);

        if (!foundSingleDesk)
        {
            return deskQueryResponse;
        }

        var desk = (Desk)deskQueryResponse.Content!;
        
        // Add the Desk ID to the parameters to enhance shift and employee queries
        if (!parameters.ContainsKey("DeskId"))
        {
            parameters["DeskId"] = desk.Id;
        }
        
        // Find Shift
        var foundSingleShift = (await _queryService.Query<Shift>(parameters))
            .ToList()
            .ValidateSingleEntry(out var shiftQueryResponse);

        if (!foundSingleShift)
        {
            return shiftQueryResponse;
        }
        
        // Find Employee
        var foundSingleEmployee = (await _queryService.Query<Employee>(parameters))
            .ToList()
            .ValidateSingleEntry(out var employeeQueryResponse);

        if (!foundSingleEmployee)
        {
            return employeeQueryResponse;
        }
        
        // Parse Exception Type
        var hasExceptionType = parameters.TryGetValue("ShiftExceptionExceptionType", out var exceptionTypeValue);
        if (!hasExceptionType)
        {
            return MissingRequiredParameters(GptRequestType.CreateShiftException, new[] { "ShiftExceptionExceptionType" });
        }

        if (exceptionTypeValue is null)
        {
            return InvalidParameterType(GptRequestType.CreateShiftException, "ShiftExceptionExceptionType",
                typeof(ExceptionType), null);
        }

        string exceptionTypeString;

        try
        {
            exceptionTypeString = Convert.ToString(exceptionTypeValue)!;
        }
        catch (InvalidCastException)
        {
            return InvalidParameterType(GptRequestType.CreateShiftException, "ShiftExceptionExceptionType", typeof(string),
                exceptionTypeValue.GetType());
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }

        ExceptionType exceptionType;

        try
        {
            exceptionType = (ExceptionType)Enum.Parse(typeof(ExceptionType), exceptionTypeString);
        }
        catch (ArgumentException)
        {
            return InvalidParameterValue(GptRequestType.CreateShiftException, "ShiftExceptionExceptionType",
                "Constraint or OffPreference or OnPreference", exceptionTypeString);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
        
        // Create Entity Parameters Dictionary With All Required Fields
        var shift = (Shift)shiftQueryResponse.Content!;
        var employee = (Employee)employeeQueryResponse.Content!;

        var entityParameters = new Dictionary<string, object>
        {
            { "Shift", shift },
            { "Employee", employee },
            { "ShiftExceptionExceptionType", exceptionType }
        };
        
        // Maybe Find Reason
        var haveReason = parameters.TryGetValue("ShiftExceptionReason", out var reasonValue);
        if (haveReason)
        {
            string reason;
            try
            {
                reason = Convert.ToString(reasonValue)!;
            }
            catch (Exception ex)
            {
                return Problem(
                    "error while trying to convert the reason parameter into string. try to see error details. " +
                    ex.Message);
            }

            entityParameters["ShiftExceptionReason"] = reason;
        }

        return Ok(entityParameters);
    }

    public override bool CreateInstance(Dictionary<string, object> parameters, out IGptResponse creationResponse)
    {
        Desk desk;
        Shift shift;
        Employee employee;
        ExceptionType exceptionType;
        var reason = "";
        
        try
        {
            desk = (Desk)parameters["Desk"];
            shift = (Shift)parameters["Shift"];
            employee = (Employee)parameters["Employee"];
            exceptionType = (ExceptionType)parameters["ShiftExceptionExceptionType"];
            
            if (parameters.TryGetValue("ShiftExceptionReason", out var reasonValue))
            {
                reason = (string)reasonValue;
            }
        }
        catch (InvalidCastException ex)
        {
            creationResponse = Problem(
                "an unhandled cast exception was thrown when trying to create the shift exception instance. " +
                ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            creationResponse =
                Problem("an unhandled exception was thrown when trying to cast the shift exception parameters. " +
                        ex.Message);
            return false;
        }

        var shiftException = new ShiftException
        {
            Shift = shift,
            Employee = employee,
            ExceptionType = exceptionType,
            Reason = reason,
            Desk = desk
        };

        creationResponse = Ok(shiftException);
        return true;
    }
}