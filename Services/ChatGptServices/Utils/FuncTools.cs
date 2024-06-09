using System.Text.Json;
using System.Text.Json.Serialization;
using SchedulerApi.Enums;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Services.ChatGptServices.Utils;

public static class FuncTools
{
    public const string StartGatherFlag = "//START GATHER//";
    public const string EndGatherFlag = "//END GATHER//";
    public const string StartJsonFlag = "//START JSON//";
    public const string EndJsonFlag = "//END JSON//";
    
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
                EndJsonFlag, 
                false
            ), 
            "[", 
            "]", 
            true
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

    public static string GetSubstringBetweenEndpoints(string fullString, string startEndpoint, string endEndpoint, bool inclusive)
    {
        var (startIndexIncluding, endIndexNotIncluding) = GetSubstringEndpointsIndexes(fullString, startEndpoint, endEndpoint);
        
        var startIndex = startIndexIncluding + (inclusive ? 0 : startEndpoint.Length);
        var endIndex = endIndexNotIncluding + (inclusive ? endEndpoint.Length : 0);
        
        return startIndex < endIndex ? fullString.Substring(startIndex, endIndex - startIndex) : "";
    }

    // public static (string, string) GetComplementaryToSubstringBetweenEndpoints(string fullString, string startEndPoint,
    //     string endEndpoint)
    // {
    //     var (startIndex, endIndex) = GetSubstringEndpointsIndexes(fullString, startEndPoint, endEndpoint);
    //     
    //     return (
    //         fullString.Substring(0, startIndex + 1),
    //         fullString.Substring(endIndex, fullString.Length - endIndex + 1)
    //         );
    // }

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
