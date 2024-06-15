using SchedulerApi.Enums;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.CommandRegistry;

public interface ICommandRegistry
{
    IGptCommand GetCommand(GptRequestType requestType);
}