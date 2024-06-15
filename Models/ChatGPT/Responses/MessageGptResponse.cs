using Newtonsoft.Json;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Responses.BaseClasses;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;

namespace SchedulerApi.Models.ChatGPT.Responses;

public class MessageGptResponse : GptResponse, IMessageGptResponse
{
    [JsonIgnore]
    public string ResponseMessage
    {
        get => Content.ToString()!;
        set => Content = new { ResponseMessage = value };
    }

    public static IMessageGptResponse NotFound(Type entityType, Dictionary<string, object> query) =>
        new MessageGptResponse
        {
            StatusCode = "404",
            ResponseMessage =
                $"{entityType} with {string.Join(",", query.Select((key, value) => $"{key} = {value}"))} not found in database."
        };
    
    public static IMessageGptResponse NotFound(Type entityType) =>
        new MessageGptResponse
        {
            StatusCode = "404",
            ResponseMessage = $"{entityType} not found in database."
        };

    public static IMessageGptResponse NotFound(string unsupportedRequestType) => new MessageGptResponse
    {
        StatusCode = "404",
        ResponseMessage = $"{unsupportedRequestType} request type does not exist."
    };

    public static IMessageGptResponse Conflict(Type entityType, Dictionary<string, object> query) =>
        new MessageGptResponse
        {
            StatusCode = "409",
            ResponseMessage =
                $"{entityType} with {string.Join(",", query.Select((key, value) => $"{key} = {value}"))} has more than one result in database. try query using entity key."
        };
    
    public static IMessageGptResponse Conflict(Type entityType) =>
        new MessageGptResponse
        {
            StatusCode = "409",
            ResponseMessage = $"{entityType} has more than one result in database. try query using entity key."
        };
    
    public static IMessageGptResponse Conflict(Type entityType, object primaryKey) =>
        new MessageGptResponse
        {
            StatusCode = "409",
            ResponseMessage = $"{entityType} with {primaryKey} primary key already exists in database."
        };

    public static IMessageGptResponse
        MissingRequiredParameters(GptRequestType requestType, string[] missingParameters) => new MessageGptResponse
    {
        StatusCode = "400",
        ResponseMessage = $"{requestType} is missing required fields {string.Join(",", missingParameters)}."
    };

    public static IMessageGptResponse InvalidParameterType(GptRequestType requestType, string parameterName,
        Type requiredType, Type? givenType) => new MessageGptResponse
    {
        StatusCode = "400",
        ResponseMessage = $"Invalid {parameterName} type for {requestType}. Expected {requestType}, given {givenType}."
    };

    public static IMessageGptResponse InvalidParameterValue(GptRequestType requestType, string parameterName,
        object requiredValue, object? givenValue) => new MessageGptResponse
    {
        StatusCode = "400",
        ResponseMessage =
            $"Invalid {parameterName} value for {requestType}. Expected {requiredValue}, got {givenValue}."
    };
    
    public static IMessageGptResponse Problem(string message = "Problem while executing the request.") => new MessageGptResponse
    {
        StatusCode = "400",
        ResponseMessage = message
    };

    public static IMessageGptResponse Forbidden(string message = "Forbidden") => new MessageGptResponse
    {
        StatusCode = "403",
        ResponseMessage = message
    };

    public static IMessageGptResponse Ok(string message = "Success") => new MessageGptResponse
    {
        StatusCode = "200",
        ResponseMessage = message
    };
    
    public static IMessageGptResponse Created(Type type, object key) => new MessageGptResponse
    {
        StatusCode = "201",
        ResponseMessage = $"{type.Name} '{key}' created successfully."
    };

    public static IMessageGptResponse NoContent(string message = "") => new MessageGptResponse
    {
        StatusCode = "204",
        ResponseMessage = message
    };
    
    public static IMessageGptResponse ServerError(
        string message = "Internal server error lead to the termination of the request. This is probably a transient error. Please try again or contact system administrator.") 
        => new MessageGptResponse
    {
        StatusCode = "500",
        ResponseMessage = message
    };
}
