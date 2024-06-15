using SchedulerApi.Models.ChatGPT.Requests.Interfaces;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling;

public interface IGptRequestHandler
{
    Task<IGptResponse> HandleRequest(IGptRequest request);
}