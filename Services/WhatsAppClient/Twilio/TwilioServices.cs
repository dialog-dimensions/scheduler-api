using System.Globalization;
using Azure.Security.KeyVault.Secrets;
using Twilio;
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
    
    public async Task TriggerCallToFileFlow(string phoneNumber, string userName, DateTime scheduleStartDateTime,
        DateTime fileWindowEndDateTime)
    {
        var windowDuration = TimeSpan.FromHours(_processParams.GetValue<double>("FileWindowDurHrs"));
        var windowEnd = DateTime.Now.Add(windowDuration);
        
        var parameters = new Dictionary<string, object>
        {
            { "name", userName },
            { "scheduleStart", scheduleStartDateTime.ToString(DateFormat, _he) },
            { "fileEndDate", windowEnd.ToString(DateFormat, _he) },
            { "fileEndTime", windowEnd.ToString("HH:mm") }
        };
        
        var execution = await ExecutionResource.CreateAsync(
            to: new PhoneNumber(PhoneNumberFormat(phoneNumber)),
            from: new PhoneNumber(SenderPhoneNumber),
            parameters: parameters,
            pathFlowSid: _flows["CallToFile"]!
        );

        Console.WriteLine(execution.Sid);
    }

    public Task TriggerAckFileFlow(string phoneNumber, DateTime fileEndDateTime, DateTime publishDateTime)
    {
        throw new NotImplementedException();
    }

    public async Task TriggerNotifyManagerFlow(string phoneNumber, string managerName, DateTime scheduleStartDateTime,
        DateTime approveWindowEndDateTime)
    {
        var headsUp = TimeSpan.FromHours(_processParams.GetValue<double>("HeadsUpDurHrs"));
        var autoPublishDateTime = scheduleStartDateTime.Subtract(headsUp);
        
        var parameters = new Dictionary<string, object>
        {
            { "name", managerName },
            { "scheduleStart", scheduleStartDateTime.ToString(DateFormat, _he) },
            { "publishDate", autoPublishDateTime.ToString(DateFormat, _he) },
            { "publishTime", autoPublishDateTime.ToString("HH:mm") }
        };
        
        var execution = await ExecutionResource.CreateAsync(
            parameters: parameters,
            to: new PhoneNumber(PhoneNumberFormat(phoneNumber)),
            from: new PhoneNumber(SenderPhoneNumber),
            pathFlowSid: _flows["ManagerApprove"]!
        );

        Console.WriteLine(execution.Sid);
    }

    public async Task TriggerPublishShiftsFlow(string phoneNumber, string employeeName, DateTime scheduleStartDateTime)
    {
        var parameters = new Dictionary<string, object>
        {
            { "name", employeeName },
            { "scheduleStart", scheduleStartDateTime.ToString(DateFormat, _he) },
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
