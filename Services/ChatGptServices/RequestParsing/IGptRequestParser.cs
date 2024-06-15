using SchedulerApi.Models.ChatGPT.Requests.Interfaces;

namespace SchedulerApi.Services.ChatGptServices.RequestParsing;

public interface IGptRequestParser
{
    IGptRequest ParseRequest(string requestString);
}