using SchedulerApi.Enums;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;

namespace SchedulerApi.Services.WhatsAppClient.Twilio;

public interface ITwilioServices
{
    Task TriggerCallToFileFlow(string phoneNumber, Desk desk, string userName, DateTime scheduleStartDateTime, DateTime fileWindowEndDateTime);
    Task TriggerAckFileFlow(string phoneNumber, Desk desk, DateTime fileEndDateTime, DateTime publishDateTime);
    Task TriggerNotifyManagerFlow(string phoneNumber, Desk desk, string managerName, DateTime scheduleStartDateTime, DateTime approveWindowEndDateTime);
    Task TriggerPublishShiftsFlow(string phoneNumber, Desk desk, string employeeName, DateTime from, DateTime to);
    Task TriggerCallToRegisterFlow(string userName, string userId, string phoneNumber);
    Task TriggerCallToFileGptFlow(string phoneNumber, string userName, Gender gender, Schedule schedule, DateTime fileWindowEndDateTime);
    Task SendFreeFormMessage(string message, string phoneNumber);
    Task TriggerPublishShiftsMediaFlow(string phoneNumber, string userName, Schedule schedule, Employee employee);
}
