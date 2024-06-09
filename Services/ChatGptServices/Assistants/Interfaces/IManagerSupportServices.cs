using SchedulerApi.Models.Entities.Workers;

namespace SchedulerApi.Services.ChatGptServices.Assistants.Interfaces;

public interface IManagerSupportServices
{
    Task ProcessIncomingMessage(Employee manager, string incomingMessage);
}