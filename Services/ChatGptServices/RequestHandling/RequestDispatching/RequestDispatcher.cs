using SchedulerApi.Models.ChatGPT.Requests.Interfaces;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Services.ChatGptServices.RequestHandling.CommandRegistry;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.RequestDispatching;

public class RequestDispatcher : IRequestDispatcher
{
    private readonly ICommandRegistry _commandRegistry;

    public RequestDispatcher(ICommandRegistry commandRegistry)
    {
        _commandRegistry = commandRegistry;
    }

    public async Task<IGptResponse> HandleRequest(IGptRequest request)
    {
        var command = _commandRegistry.GetCommand(request.GptRequestType);
        return await command.Execute(request.Parameters);
    }
}
