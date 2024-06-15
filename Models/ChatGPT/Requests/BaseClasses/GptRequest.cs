using Newtonsoft.Json;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Requests.Interfaces;

namespace SchedulerApi.Models.ChatGPT.Requests.BaseClasses;

public class GptRequest : IGptRequest
{
    [JsonProperty("RequestType")] 
    public string RequestTypeName { get; set; }
    
    public GptRequestType GptRequestType => (GptRequestType)Enum.Parse(typeof(GptRequestType), RequestTypeName);
    
    
    [JsonProperty("Parameters")]
    public Dictionary<string, object?> Parameters { get; set; }
}
