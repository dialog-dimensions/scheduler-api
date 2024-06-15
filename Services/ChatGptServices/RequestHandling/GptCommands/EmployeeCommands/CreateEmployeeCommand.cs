using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;
using static SchedulerApi.Services.ChatGptServices.Utils.ValidationUtils;
using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;
using static SchedulerApi.Models.ChatGPT.Responses.EntityGptResponse;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.EmployeeCommands;

public class CreateEmployeeCommand : CreateCommand<Employee>
{
    private readonly IQueryService _queryService;
    
    public CreateEmployeeCommand(IEmployeeRepository employeeRepository, IQueryService queryService) : base(employeeRepository)
    {
        _queryService = queryService;
    }

    public override async Task<IGptResponse> ValidateEntityParameters(Dictionary<string, object> parameters)
    {
        // Validate ID
        if (!ValidateEmployeeParameter<int>("EmployeeId", out var idValidationResponse))
        {
            return idValidationResponse;
        }
        
        // Validate Name
        if (!ValidateEmployeeParameter<string>("EmployeeName", out var nameParameterResponse))
        {
            return nameParameterResponse;
        }
        
        // Validate Role
        if (!ValidateEmployeeParameter<string>("EmployeeRole", out var roleParameterResponse))
        {
            return roleParameterResponse;
        }
        
        // Validate Unit
        var foundSingleUnit = (await _queryService.Query(typeof(Unit), parameters))
            .ToList()
            .ValidateSingleEntry(out var validateUnitResponse);
        
        if (!foundSingleUnit)
        {
            return validateUnitResponse;
        }

        // Return the entity parameters in the response
        var entityParameters = new Dictionary<string, object>
        {
            { "EmployeeId", idValidationResponse.Content! },
            { "EmployeeName", nameParameterResponse.Content! },
            { "EmployeeRole", roleParameterResponse.Content! },
            { "Unit", validateUnitResponse.Content! }
        };

        return Ok(entityParameters);
        
        bool ValidateEmployeeParameter<T>(string paramName, out IGptResponse paramValidationResponse, 
            bool nullable = false, T[]? allowedValues = null) => ValidateParameter(GptRequestType.CreateEmployee, 
            parameters, paramName, out paramValidationResponse, nullable, allowedValues);
    }

    public override bool CreateInstance(Dictionary<string, object> parameters, out IGptResponse creationResponse)
    {
        int id;
        string name;
        string role;
        Unit unit;
        
        try
        {
            id = (int)parameters["EmployeeId"];
            name = (string)parameters["EmployeeName"];
            role = (string)parameters["EmployeeRole"];
            unit = (Unit)parameters["Unit"];
        }
        catch (InvalidCastException ex)
        {
            creationResponse =
                Problem("an unhandled validation exception was thrown during cast of entity properties. " + ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            creationResponse =
                Problem("an unhandled exception was thrown during cast of entity properties. try again. see details. " + ex.Message);
            return false;
        }

        creationResponse = Ok(new Employee
        {
            Id = id,
            Name = name,
            Role = role,
            Unit = unit
        });
        return true;
    }
}
