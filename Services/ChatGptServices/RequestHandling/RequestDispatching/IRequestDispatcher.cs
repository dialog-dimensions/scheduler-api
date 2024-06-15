using SchedulerApi.Models.ChatGPT.Requests.Interfaces;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.RequestDispatching;

public interface IRequestDispatcher
{
    Task<IGptResponse> HandleRequest(IGptRequest request);
}