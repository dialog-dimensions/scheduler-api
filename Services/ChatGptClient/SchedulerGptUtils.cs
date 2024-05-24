using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web.Helpers;
using Newtonsoft.Json.Converters;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Services.ChatGptClient;

public static class SchedulerGptUtils
{
    public const string StartGatherFlag = "//*START GATHER*//";
    public const string EndGatherFlag = "//*END GATHER*//";
    public const string StartJsonFlag = "//*START JSON*//";
    public const string EndJsonFlag = "//*END JSON*//";
    
    public static string InitialStringBuilder(
        Schedule schedule, Employee employee, Dictionary<string, string>? otherInstructions = null) =>
        $"{{" +
        $"    \"ScheduleDetails\": {{" +
        $"        \"StartDateTime\": \"{schedule.StartDateTime}\"," +
        $"        \"EndDateTime\": \"{schedule.EndDateTime}\"," +
        $"        \"ShiftDurationHrs\": \"{schedule.ShiftDuration}\"" +
        $"    }}" +
        $"    \"DeskDetails\": {{" +
        $"        \"DeskId\": \"{schedule.DeskId}\"," +
        $"        \"DeskName\": \"{schedule.Desk.Name}\"" +
        $"    }}," +
        $"    \"EmployeeDetails\": {{" +
        $"        \"Name\": \"{employee.Name}\"," +
        $"        \"Id\": \"{employee.Id}\"," +
        $"        \"Gender\": \"{employee.Gender}\"" +
        $"    }}," +
        $"    \"OtherInstructions\": {{" +
        $"{string.Join(",", otherInstructions?.Select(kv => $"        \"{kv.Key}\": \"{kv.Value}\"") ?? Array.Empty<string>())}" +
        $"    }}" +
        $"}}";

    public static ShabtzanGptConversationState? AnalyzeConversationState(string message)
    {
        ShabtzanGptConversationState? result = null;

        if (message.Contains(StartGatherFlag))
        {
            result = ShabtzanGptConversationState.Gathering;
        }

        if (message.Contains(StartJsonFlag))
        {
            result = ShabtzanGptConversationState.JsonTransmission;
        }

        if (message.Contains(EndJsonFlag))
        {
            result = ShabtzanGptConversationState.JsonDetected;
        }

        return result;
    }

    public static IEnumerable<ShiftException> GetShiftExceptions(string message)
    {
        // Trim Json Bit from Json Section
        var jsonString = GetSubstringBetweenEndpoints(
            GetSubstringBetweenEndpoints(
                message, 
                StartJsonFlag, 
                EndJsonFlag
            ), 
            "[", 
            "]"
        );
        
        // Deserialize Json String
        try
        {
            var options = new JsonSerializerOptions()
            {
                Converters = { new JsonStringEnumConverter() }
            };

            return JsonSerializer.Deserialize<IEnumerable<ShiftException>>(jsonString, options) ?? [];
        }
        catch
        {
            return [];
        }
    }

    private static string GetSubstringBetweenEndpoints(string fullString, string startEndpoint, string endEndpoint)
    {
        var (startIndex, endIndex) = GetSubstringEndpointsIndexes(fullString, startEndpoint, endEndpoint);
        return fullString.Substring(startIndex, endIndex - startIndex + 1);
    }

    public static (string, string) GetComplementaryToSubstringBetweenEndpoints(string fullString, string startEndPoint,
        string endEndpoint)
    {
        var (startIndex, endIndex) = GetSubstringEndpointsIndexes(fullString, startEndPoint, endEndpoint);
        
        return (
            fullString.Substring(0, startIndex + 1),
            fullString.Substring(endIndex, fullString.Length - endIndex + 1)
            );
    }

    public static string GetSubstringPriorToFlag(string message, string flag)
    {
        var flagIndex = message.IndexOf(flag, StringComparison.Ordinal);
        if (flagIndex == -1) return "";

        return message.Substring(0, flagIndex);
    }

    public static (int, int) GetSubstringEndpointsIndexes(string fullString, string startEndpoint, string endEndpoint)
    {
        return (
            fullString.IndexOf(startEndpoint, StringComparison.Ordinal),
            fullString.IndexOf(endEndpoint, StringComparison.Ordinal)
            );
    }
}
