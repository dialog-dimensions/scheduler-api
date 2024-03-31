namespace SchedulerApi.Services.WhatsAppClient.Twilio;

public interface ITwilioServices
{
    Task TriggerCallToFileFlow(string phoneNumber, string userName, DateTime scheduleStartDateTime, DateTime fileWindowEndDateTime);
    Task TriggerAckFileFlow(string phoneNumber, DateTime fileEndDateTime, DateTime publishDateTime);
    Task TriggerNotifyManagerFlow(string phoneNumber, string managerName, DateTime scheduleStartDateTime, DateTime approveWindowEndDateTime);
    Task TriggerPublishShiftsFlow(string phoneNumber, string employeeName, DateTime from, DateTime to);
    Task TriggerCallToRegisterFlow(string userName, string userId, string phoneNumber);
}
