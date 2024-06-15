using System.Runtime.CompilerServices;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands;

public interface IGptCommand
{
    Task<IGptResponse> Execute(Dictionary<string, object> parameters);
}