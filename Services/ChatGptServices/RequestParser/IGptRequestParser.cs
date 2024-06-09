using SchedulerApi.Models.ChatGPT.Requests.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestParser;

public interface IGptRequestParser
{
    IGptRequest ParseRequest(string requestString);
}