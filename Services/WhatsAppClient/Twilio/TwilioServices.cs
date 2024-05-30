using System.Globalization;
using Azure.Security.KeyVault.Secrets;
using SchedulerApi.Enums;
using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ImageGenerationServices.ScheduleToImageStorage;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Studio.V1.Flow;
using Twilio.Types;

namespace SchedulerApi.Services.WhatsAppClient.Twilio;

public class TwilioServices : ITwilioServices
{
    private readonly IConfigurationSection _twilioConfig;
    private readonly IConfigurationSection _flows;
    private readonly IConfigurationSection _processParams;

    private const string DateFormat = "dddd, dd.MM";
    private readonly CultureInfo _he = new("he-IL");
    private static string PhoneNumberFormat(string purePhoneNumber) => $"+972{purePhoneNumber[1..]}";

    private static string WhatsAppPhoneNumberFormat(string purePhoneNumber) =>
        $"whatsapp:{PhoneNumberFormat(purePhoneNumber)}";
    
    private string SenderPhoneNumber => _twilioConfig["Account:ServiceSid"]!;
    
    public TwilioServices(IConfiguration configuration, SecretClient secretClient)
    {
        _twilioConfig = configuration.GetSection("Twilio");
        _flows = _twilioConfig.GetSection("Flows");
        _processParams = configuration.GetSection("Params:Processes:AutoSchedule");

        var kvTwilioSecretNames = configuration.GetSection("KeyVault:SecretNames:Twilio");
        var sid = secretClient.GetSecret(kvTwilioSecretNames["AccountSid"]).Value.Value;
        var authToken = secretClient.GetSecret(kvTwilioSecretNames["AccountAuthToken"]).Value.Value;
        
        TwilioClient.Init(
            sid, 
            authToken
            );
    }
    
    public async Task TriggerCallToRegisterFlow(string userName, string userId, string phoneNumber)
    {
        var parameters = new Dictionary<string, object>
        {
            { "name", userName },
            { "query", $"?userId={userId}&phoneNumber={phoneNumber}&redirect=login?userId={userId}"}
        };

        var execution = await ExecutionResource.CreateAsync(
            parameters: parameters,
            to: new PhoneNumber(PhoneNumberFormat(phoneNumber)),
            from: new PhoneNumber(SenderPhoneNumber),
            pathFlowSid: _flows["CallToRegister"]!
        );
        
        Console.WriteLine(execution.Sid);
    }

    public async Task TriggerCallToFileGptFlow(string phoneNumber, string userName, Gender gender, Schedule schedule, DateTime fileWindowEndDateTime)
    {
        var generalNote =
            "חשוב לשים לב שחלון ההגשה נסגר ב " +
            $"{fileWindowEndDateTime.ToString(DateFormat, _he)}" +
            " בשעה " +
            $"{fileWindowEndDateTime.ToString("HH:mm")}";
        
        var parameters = new Dictionary<string, object>
        {
            { "name", userName },
            { "scheduleStartDate", schedule.StartDateTime.ToString(DateFormat, _he) },
            { "scheduleEndDate", schedule.EndDateTime.ToString(DateFormat, _he) },
            { "generalNote", generalNote },
            { "gender", gender.ToString()}
        };

        var execution = await ExecutionResource.CreateAsync(
            parameters: parameters,
            to: new PhoneNumber(WhatsAppPhoneNumberFormat(phoneNumber)),
            from: new PhoneNumber(SenderPhoneNumber),
            pathFlowSid: _flows["CallToFileGpt"]!
        );

        Console.WriteLine(execution.Sid);
    }

    public async Task SendFreeFormMessage(string content, string phoneNumber)
    {
        await MessageResource.CreateAsync(
            body: content,
            from: new PhoneNumber(SenderPhoneNumber),
            to: new PhoneNumber(WhatsAppPhoneNumberFormat(phoneNumber))
        );
    }

    public async Task TriggerPublishShiftsMediaFlow(string phoneNumber, string userName, Schedule schedule, Employee employee)
    {
        var deskName = schedule.Desk.Name;
        var blobName = IScheduleImageService.GetBlobName(schedule, employee);

        var parameters = new Dictionary<string, object>
        {
            { "name", userName },
            { "deskName", deskName },
            { "blobName", blobName }
        };

        var execution = await ExecutionResource.CreateAsync(
            parameters: parameters,
            to: new PhoneNumber(PhoneNumberFormat(phoneNumber)),
            from: new PhoneNumber(SenderPhoneNumber),
            pathFlowSid: _flows["PublishShiftsMedia"]!
        );

        Console.WriteLine(execution.Sid);
    }

    public async Task TriggerCallToFileFlow(string phoneNumber, Desk desk, string userName, DateTime scheduleStartDateTime,
        DateTime fileWindowEndDateTime)
    {
        var parameters = new Dictionary<string, object>
        {
            { "name", userName },
            { "scheduleStart", scheduleStartDateTime.ToString(DateFormat, _he) },
            { "fileEndDate", fileWindowEndDateTime.ToString(DateFormat, _he) },
            { "fileEndTime", fileWindowEndDateTime.ToString("HH:mm") },
            { "deskId", desk.Id },
            { "deskName", desk.Name }
        };
        
        var execution = await ExecutionResource.CreateAsync(
            to: new PhoneNumber(PhoneNumberFormat(phoneNumber)),
            from: new PhoneNumber(SenderPhoneNumber),
            parameters: parameters,
            pathFlowSid: _flows["CallToFile"]!
        );

        Console.WriteLine(execution.Sid);
    }

    public async Task TriggerAckFileFlow(string phoneNumber, Desk desk, DateTime fileEndDateTime, DateTime publishDateTime)
    {
        var parameters = new Dictionary<string, object>
        {
            { "publishDate", publishDateTime.ToString(DateFormat, _he) },
            { "fileWindowEndDate", fileEndDateTime.ToString(DateFormat, _he) },
            { "fileWindowEndTime", fileEndDateTime.ToString("HH:mm") },
            { "deskName", desk.Name }
        };
        
        var execution = await ExecutionResource.CreateAsync(
            to: new PhoneNumber(PhoneNumberFormat(phoneNumber)),
            from: new PhoneNumber(SenderPhoneNumber),
            parameters: parameters,
            pathFlowSid: _flows["AcknowledgeFile"]!
        );

        Console.WriteLine(execution.Sid);
    }

    public async Task TriggerNotifyManagerFlow(string phoneNumber, Desk desk, string managerName, DateTime scheduleStartDateTime,
        DateTime approveWindowEndDateTime)
    {
        var headsUp = TimeSpan.FromHours(_processParams.GetValue<double>("HeadsUpDurHrs"));
        var autoPublishDateTime = scheduleStartDateTime.Subtract(headsUp);

        var scheduleRoute = $"{desk.Id}/next";
        
        var parameters = new Dictionary<string, object>
        {
            { "name", managerName },
            { "scheduleStart", scheduleStartDateTime.ToString(DateFormat, _he) },
            { "publishDate", autoPublishDateTime.ToString(DateFormat, _he) },
            { "publishTime", autoPublishDateTime.ToString("HH:mm") },
            { "scheduleRoute", scheduleRoute },
            { "deskName", desk.Name }
        };
        
        var execution = await ExecutionResource.CreateAsync(
            parameters: parameters,
            to: new PhoneNumber(PhoneNumberFormat(phoneNumber)),
            from: new PhoneNumber(SenderPhoneNumber),
            pathFlowSid: _flows["ManagerApprove"]!
        );

        Console.WriteLine(execution.Sid);
    }

    public async Task TriggerPublishShiftsFlow(string phoneNumber, Desk desk, string employeeName, DateTime from, DateTime to)
    {
        var scheduleRoute = $"{desk.Id}/next";
        
        var parameters = new Dictionary<string, object>
        {
            { "name", employeeName },
            { "scheduleStart", from.ToString("dd.MM") },
            { "scheduleEnd", to.ToString("dd.MM") },
            { "scheduleRoute", scheduleRoute },
            { "deskName", desk.Name }
        };

        var execution = await ExecutionResource.CreateAsync(
            parameters: parameters,
            to: new PhoneNumber(PhoneNumberFormat(phoneNumber)),
            from: new PhoneNumber(SenderPhoneNumber),
            pathFlowSid: _flows["PublishShifts"]!
        );
        
        Console.WriteLine(execution.Sid);
    }
    
    
    
    
    
    
    
    
    public async Task TriggerTestFlow()
    {
        var testPhoneNumber = "0546296622";

        var execution = await ExecutionResource.CreateAsync(
            to: new PhoneNumber($"whatsapp:+972{testPhoneNumber[1..]}"),
            from: new PhoneNumber(_twilioConfig["Account:ServiceSid"]!),
            pathFlowSid: _twilioConfig["Flows:Test"]!
        );

        Console.WriteLine(execution.Sid);
    }
}
