using SchedulerApi.Models.ChatGPT.Requests.BaseClasses;
using SchedulerApi.Models.ChatGPT.Responses.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandlers;

public interface IGptRequestHandler
{
    Task<IGptResponse> HandleRequest(IGptRequest request);
}