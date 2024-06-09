using System.Text;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Requests.BaseClasses;
using SchedulerApi.Models.ChatGPT.Responses;
using SchedulerApi.Models.ChatGPT.Responses.BaseClasses;
using SchedulerApi.Models.DTOs.ChatGpt.EmployeeManagementAssistant;
using SchedulerApi.Models.DTOs.ChatGpt.ScheduleManagementAssistant;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;
using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ChatGptServices.RequestHandlers;

public class GptRequestHandler : IGptRequestHandler
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IDeskRepository _deskRepository;
    private readonly IUnitRepository _unitRepository;

    public GptRequestHandler(IShiftRepository shiftRepository, IEmployeeRepository employeeRepository, IScheduleRepository scheduleRepository, IDeskRepository deskRepository, IUnitRepository unitRepository)
    {
        _shiftRepository = shiftRepository;
        _employeeRepository = employeeRepository;
        _scheduleRepository = scheduleRepository;
        _deskRepository = deskRepository;
        _unitRepository = unitRepository;
    }

    private static IMessageGptResponse SuccessGptResponse(string message = "Success") => new MessageGptResponse
    {
        StatusCode = "200",
        ResponseMessage = message
    };
    
    private IMessageGptResponse MissingParameterGptResponse(string parameterName) => new MessageGptResponse
    {
        StatusCode = "400",
        ResponseMessage = $"Missing {parameterName} parameter in request body."
    };

    private IMessageGptResponse InvalidParameterTypeGptResponse(string parameterName, Type requiredType,
        Type givenType) => new MessageGptResponse
    {
        StatusCode = "400",
        ResponseMessage = $"Invalid {parameterName} type. Required {requiredType.Name}, given {givenType.Name}."
    };

    private IMessageGptResponse InvalidParameterValueGptResponse(string parameterName, object givenValue,
        object? expectedValue = null)
    {
        var responseMessage = new StringBuilder();
        
        responseMessage.Append($"Invalid {parameterName} value.");
        if (expectedValue is not null)
        {
            responseMessage.Append($" Expected {expectedValue}.");
        }
        responseMessage.Append($" Given {givenValue}.");
        
        return new MessageGptResponse
        {
            StatusCode = "400",
            ResponseMessage = responseMessage.ToString()
        };
    }

    private IMessageGptResponse NotFoundGptResponse(string parameterName, object value, Type type) => new MessageGptResponse
    {
        StatusCode = "404",
        ResponseMessage = $"Invalid {parameterName}. {type.Name} '{value}' not found in database."
    };
    
    private IMessageGptResponse ConflictGptResponse(string parameterName, object value, Type type) => new MessageGptResponse
    {
        StatusCode = "409",
        ResponseMessage = $"Conflicting {parameterName}. More than one {type.Name} records with '{value}' found in database."
    };
    
    public async Task<IGptResponse> HandleRequest(IGptRequest request)
    {
        var handler = GetHandler(request.GptRequestType);
        return await handler.Invoke(request.Parameters);
    }

    private Func<Dictionary<string, object>, Task<IGptResponse>> GetHandler(GptRequestType requestType) => 
        requestType switch
    {
        GptRequestType.CreateEmployee => HandleCreateEmployeeRequestAsync,
        GptRequestType.CreateDeskAssignment => HandleCreateDeskAssignmentRequestAsync,
        GptRequestType.ReadEmployee => HandleReadEmployeeRequestAsync,
        GptRequestType.ReadSchedule => HandleReadScheduleRequestAsync,
        GptRequestType.AssignShift => HandleAssignShiftRequestAsync,
        GptRequestType.SwapShifts => HandleSwapShiftsRequestAsync,
        GptRequestType.DeleteDeskAssignment => HandleDeleteDeskAssignmentRequestAsync,
        // GptRequestType.ReadDeskAssignment => HandleReadDeskAssignmentRequestAsync, TODO: Add ReadDeskEmployees and ReadEmployeeDesks
        // GptRequestType.CreateShiftException => HandleCreateShiftExceptionRequestAsync,
        // GptRequestType.ReadShift => HandleReadShiftRequestAsync,
        // GptRequestType.ReadShiftException => HandleReadShiftExceptionRequestAsync,
        // GptRequestType.GetScheduleShiftExceptions => HandleGetScheduleShiftExceptionsRequestAsync,
        // GptRequestType.PatchEmployee => HandlePatchEmployeeRequestAsync,
        // GptRequestType.PatchShiftException => HandlePatchShiftExceptionRequestAsync,
        // GptRequestType.DeleteShiftException => HandleDeleteShiftExceptionRequestAsync,
        _ => HandleUnrecognizedRequestType
    };

    private async Task<IGptResponse> HandleUnrecognizedRequestType(Dictionary<string, object> arg)
    {
        return new MessageGptResponse { StatusCode = "404", ResponseMessage = "Request type not recognized." };
    }
    
    private async Task<IGptResponse> HandleReadScheduleRequestAsync(Dictionary<string, object> arg)
    {
        var requiredFields = new[]
        {
            ("DeskId", typeof(string)),
            ("StartDateTime", typeof(DateTime))
        };

        var validationResponse = ValidateFields(requiredFields, arg);
        if (validationResponse is not null)
        {
            return validationResponse;
        }

        try
        {
            var deskId = (string)arg["DeskId"];
            var startDateTime = (DateTime)arg["StartDateTime"];
            var scheduleData = await _scheduleRepository.GetScheduleData(deskId, startDateTime);
            
            return new EntityGptResponse<ScheduleData, ScheduleDataDto>
            {
                StatusCode = "200",
                Entity = ScheduleDataDto.FromEntity(scheduleData)
            };
        }
        catch (Exception ex)
        {
            return new MessageGptResponse
            {
                StatusCode = "500",
                ResponseMessage = ex.Message
            };
        }
    }

    private async Task<IGptResponse> HandleAssignShiftRequestAsync(Dictionary<string, object> arg)
    {
        var requiredFields = new[]
        {
            ("DeskId", typeof(string)), 
            ("ShiftStartDateTime", typeof(DateTime)), 
            ("EmployeeId", typeof(int))
        };

        var validationResponse = ValidateFields(requiredFields, arg);
        if (validationResponse is not null)
        {
            return validationResponse;
        }

        try
        {
            var deskId = (string)arg["DeskId"];
            var shiftStartDateTime = (DateTime)arg["ShiftStartDateTime"];
            var employeeId = (int)arg["EmployeeId"];

            var employee = await _employeeRepository.ReadAsync(employeeId);
            if (employee is null)
            {
                return NotFoundGptResponse("EmployeeId", employeeId, typeof(Employee));
            }

            await _shiftRepository.UpdateShiftEmployeeAsync(deskId, shiftStartDateTime, employee);
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException)
            {
                return new MessageGptResponse
                {
                    StatusCode = "404",
                    ResponseMessage = ex.Message
                };
            }

            return new MessageGptResponse
            {
                StatusCode = "500",
                ResponseMessage = ex.Message
            };
        }
        
        return SuccessGptResponse();
    }

    private IMessageGptResponse? ValidateFields((string, Type)[] requiredFields, Dictionary<string, object> parameters)
    {
        foreach (var (field, type) in requiredFields)
        {
            if (!parameters.ContainsKey(field))
            {
                return MissingParameterGptResponse(field);
            }

            var givenType = parameters[field].GetType();
            if (givenType != type)
            {
                return InvalidParameterTypeGptResponse(field, type, givenType);
            }
        }

        return null;
    }
    
    private async Task<IGptResponse> HandleSwapShiftsRequestAsync(Dictionary<string, object> arg)
    {
        var requiredFields = new[]
        {
            ("DeskId", typeof(string)),
            ("FirstShiftStartDateTime", typeof(DateTime)),
            ("SecondShiftStartDateTime", typeof(DateTime))
        };

        var validationResponse = ValidateFields(requiredFields, arg);
        if (validationResponse is not null)
        {
            return validationResponse;
        }

        try
        {
            var deskId = (string)arg["DeskId"];
            var firstShiftStartDateTime = (DateTime)arg["FirstShiftStartDateTime"];
            var secondShiftStartDateTime = (DateTime)arg["SecondShiftStartDateTime"];

            var firstShift = await _shiftRepository.ReadAsync((deskId, firstShiftStartDateTime));
            var secondShift = await _shiftRepository.ReadAsync((deskId, secondShiftStartDateTime));

            if (firstShift is null)
            {
                return NotFoundGptResponse("ShiftStartDateTime", firstShiftStartDateTime, typeof(Shift));
            }

            if (secondShift is null)
            {
                return NotFoundGptResponse("ShiftStartDateTime", secondShiftStartDateTime, typeof(Shift));
            }

            if (firstShift.ScheduleStartDateTime != secondShift.ScheduleStartDateTime)
            {
                return new MessageGptResponse
                {
                    StatusCode = "400",
                    ResponseMessage = "Cannot swap shifts from different schedules."
                };
            }

            if (firstShift.ScheduleStartDateTime < DateTime.Now)
            {
                return new MessageGptResponse
                {
                    StatusCode = "400",
                    ResponseMessage = "Cannot change shifts in past or ongoing schedules."
                };
            }

            if (firstShift.EmployeeId is null || firstShift.EmployeeId == 0)
            {
                return new MessageGptResponse
                {
                    StatusCode = "400",
                    ResponseMessage = $"Shift {firstShift} is unassigned. Unable to swap unassigned shift."
                };
            }

            if (secondShift.EmployeeId is null || firstShift.EmployeeId == 0)
            {
                return new MessageGptResponse
                {
                    StatusCode = "400",
                    ResponseMessage = $"Shift {secondShift} is unassigned. Unable to swap unassigned shift."
                };
            }

            var firstEmployee = firstShift.Employee!;
            var secondEmployee = secondShift.Employee!;
            await _shiftRepository.UpdateShiftEmployeeAsync(deskId, firstShiftStartDateTime, secondEmployee);
            await _shiftRepository.UpdateShiftEmployeeAsync(deskId, secondShiftStartDateTime, firstEmployee);
            return SuccessGptResponse(
                $"{firstEmployee.Name} assigned to {secondShiftStartDateTime} and {secondEmployee.Name} assigned to {firstShiftStartDateTime}");
        }
        catch (Exception ex)
        {
            return new MessageGptResponse
            {
                StatusCode = "500",
                ResponseMessage = ex.Message
            };
        }
    }

    private async Task<IGptResponse> HandleDeleteDeskAssignmentRequestAsync(Dictionary<string, object> arg)
    {
        var requiredFields = new[]
        {
            ("DeskId", typeof(string)),
            ("EmployeeId", typeof(int))
        };

        var validationMessage = ValidateFields(requiredFields, arg);
        if (validationMessage is not null)
        {
            return validationMessage;
        }

        try
        {
            var deskId = (string)arg["DeskId"];
            var employeeId = (int)arg["EmployeeId"];

            var desk = await _deskRepository.ReadAsync(deskId);
            if (desk is null)
            {
                return NotFoundGptResponse("DeskID", deskId, typeof(Desk));
            }

            var employee = await _employeeRepository.ReadAsync(employeeId);
            if (employee is null)
            {
                return NotFoundGptResponse("EmployeeID", employeeId, typeof(Employee));
            }

            await _deskRepository.RemoveDeskAssignment(desk, employee);
            return SuccessGptResponse();
        }
        catch (Exception ex)
        {
            if (ex is KeyNotFoundException)
            {
                return new MessageGptResponse
                {
                    StatusCode = "404",
                    ResponseMessage = "Desk assignment already not exists."
                };
            }

            return new MessageGptResponse
            {
                StatusCode = "500",
                ResponseMessage = ex.Message
            };
        }
    }
    
    private async Task<IGptResponse> HandleCreateEmployeeRequestAsync(Dictionary<string, object> parameters)
    {
        var requiredFields = new[]
        {
            ("Id", typeof(int)),
            ("Name", typeof(string)),
            ("Role", typeof(string)),
            ("UnitId", typeof(string))
        };

        var validationResponse = ValidateFields(requiredFields, parameters);
        if (validationResponse is not null)
        {
            return validationResponse;
        }

        try
        {
            var id = (int)parameters["Id"];
            var name = (string)parameters["Name"];
            var role = (string)parameters["Role"];
            var unitId = (string)parameters["UnitId"];

            var unit = await _unitRepository.ReadAsync(unitId);
            if (unit is null)
            {
                return NotFoundGptResponse("UnitID", unitId, typeof(Unit));
            }

            if (role is not ("Employee" or "Admin" or "Manager"))
            {
                return InvalidParameterValueGptResponse("role", role, "Employee or Manager or Admin");
            }

            var employee = new Employee
            {
                Id = id,
                Name = name,
                Role = role,
                Unit = unit
            };

            if (parameters.ContainsKey("Active"))
            {
                var activeValue = parameters["Active"];
                if (activeValue is not bool active)
                {
                    return InvalidParameterTypeGptResponse("Active", typeof(bool), activeValue.GetType());
                }

                employee.Active = active;
            }

            if (parameters.ContainsKey("Balance"))
            {
                var balanceValue = parameters["Balance"];
                if (balanceValue is not double balance)
                {
                    return InvalidParameterTypeGptResponse("Balance", typeof(double), balanceValue.GetType());
                }

                employee.Balance = balance;
            }

            if (parameters.ContainsKey("DifficultBalance"))
            {
                var difficultBalanceValue = parameters["DifficultBalance"];
                if (difficultBalanceValue is not double difficultBalance)
                {
                    return InvalidParameterTypeGptResponse("DifficultBalance", typeof(double),
                        difficultBalanceValue.GetType());
                }

                employee.DifficultBalance = difficultBalance;
            }

            if (parameters.ContainsKey("Gender"))
            {
                var genderValue = parameters["Gender"];
                if (genderValue is not string genderString)
                {
                    return InvalidParameterTypeGptResponse("Gender", typeof(string), genderValue.GetType());
                }

                var genderConversion = Enum.TryParse(typeof(Gender), genderString, out var gender);
                if (!genderConversion)
                {
                    return InvalidParameterValueGptResponse("Gender", genderString, "Male or Female or Unknown");
                }

                employee.Gender = (Gender)gender!;
            }

            await _employeeRepository.CreateAsync(employee);
            return SuccessGptResponse();
        }
        catch (Exception ex)
        {
            return new MessageGptResponse
            {
                StatusCode = "500",
                ResponseMessage = ex.Message
            };
        }
    }

    private async Task<IGptResponse> HandleCreateDeskAssignmentRequestAsync(
        Dictionary<string, object> parameters)
    {
        var requiredFields = new[]
        {
            ("DeskId", typeof(string)),
            ("EmployeeId", typeof(int))
        };

        var validationResponse = ValidateFields(requiredFields, parameters);
        if (validationResponse is not null)
        {
            return validationResponse;
        }

        try
        {
            var deskId = (string)parameters["DeskId"];
            var employeeId = (int)parameters["EmployeeId"];

            var desk = await _deskRepository.ReadAsync(deskId);
            if (desk is null)
            {
                return NotFoundGptResponse("DeskID", deskId, typeof(Desk));
            }

            var employee = await _employeeRepository.ReadAsync(employeeId);
            if (employee is null)
            {
                return NotFoundGptResponse("EmployeeID", employeeId, typeof(Employee));
            }

            await _deskRepository.AddDeskAssignment(desk, employee);
            return SuccessGptResponse();
        }
        catch (Exception ex)
        {
            return new MessageGptResponse
            {
                StatusCode = "500",
                ResponseMessage = ex.Message
            };
        }
    }
    
    
    private async Task<IGptResponse> HandleReadEmployeeRequestAsync(Dictionary<string, object> parameters)
    {
        if (!parameters.ContainsKey("Id") && (!parameters.ContainsKey("Name") || !parameters.ContainsKey("UnitId")))
        {
            return MissingParameterGptResponse("ID, or Name and Unit ID");
        }

        if (parameters.ContainsKey("Id"))
        {
            var idValue = parameters["Id"];
            if (idValue is not int id)
            {
                return InvalidParameterTypeGptResponse("ID", typeof(int), idValue.GetType());
            }

            var employee = await _employeeRepository.ReadAsync(id);
            if (employee is null)
            {
                return NotFoundGptResponse("ID", id, typeof(Employee));
            }

            return new EntityGptResponse<Employee, EmployeeDto>
            {
                StatusCode = "200",
                Entity = EmployeeDto.FromEntity(employee)
            };
        }

        else
        {
            var nameValue = parameters["Name"];
            var unitIdValue = parameters["UnitId"];

            if (nameValue is not string name)
            {
                return InvalidParameterTypeGptResponse("Name", typeof(string), nameValue.GetType());
            }

            if (unitIdValue is not string unitId)
            {
                return InvalidParameterTypeGptResponse("UnitID", typeof(string), unitIdValue.GetType());
            }

            var matchedEmployees = (await _employeeRepository.FindByNameAndUnitId(name, unitId)).ToList();
            if (matchedEmployees.Count == 0)
            {
                return NotFoundGptResponse("Name and UnitID", (name, unitId), typeof(Employee));
            }

            if (matchedEmployees.Count > 1)
            {
                return ConflictGptResponse("Name and UnitID", (name, unitId), typeof(Employee));
            }

            return new EntityGptResponse<Employee, EmployeeDto>
            {
                StatusCode = "200",
                Entity = EmployeeDto.FromEntity(matchedEmployees[0])
            };
        }
    }
}