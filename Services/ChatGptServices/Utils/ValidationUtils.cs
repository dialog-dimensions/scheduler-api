using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Responses.BaseClasses;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;
using static SchedulerApi.Models.ChatGPT.Responses.EntityGptResponse;

namespace SchedulerApi.Services.ChatGptServices.Utils;

public static class ValidationUtils
{
    public static bool ValidateSingleEntry<T>(this List<T> list, out IGptResponse response)
    {
        if (list.Count == 1)
        {
            response = new GptResponse
            {
                StatusCode = "200",
                Content = list[0]!
            };

            return true;
        }

        if (list.Count > 1)
        {
            response = Conflict(typeof(T));
        }

        else
        {
            response = NotFound(typeof(T));
        }

        return false;
    }

    public static bool ValidateParameter<T>(GptRequestType requestType, Dictionary<string, object> parameters, string paramName,
        out IGptResponse response, bool nullable = false, T[]? allowedValues = null)
    {
        // Try Get Parameter
        var hasParameter = parameters.TryGetValue(paramName, out var paramValue);
        
        // Validate Parameter Exists
        if (!hasParameter)
        {
            response = nullable ? 
                NoContent() : 
                MissingRequiredParameters(requestType, new[] { paramName });

            return nullable;
        }
        
        // Handle Null Values
        if (paramValue is null)
        {
            response = nullable ? 
                NoContent() : 
                InvalidParameterType(requestType, paramName, typeof(T), null);

            return nullable;
        }
        
        T param;
        
        // Validate Parameter Type
        try
        {
            param = (T)paramValue!;
        }
        
        catch (Exception ex)
        {
            if (ex is InvalidCastException)
            {
                response = InvalidParameterType(requestType, paramName, typeof(T), paramValue!.GetType());
            }

            else
            {
                response = Problem(ex.Message);
            }

            return false;
        }
        
        // Validate Parameter Value
        var isAllowedValue = allowedValues is null ? true : allowedValues.Contains(param);
        
        response = isAllowedValue ? 
            Ok(param) : 
            InvalidParameterValue(requestType, paramName, allowedValues!, param);

        return isAllowedValue;
    }
}
