using SchedulerApi.Models.ChatGPT.Responses.Interfaces;

namespace SchedulerApi.Services.Workflows.Jobs;

public interface IAutoScheduleProcessJobServices
{
    public static string GetScannerJobId(string deskId) => $"{deskId}_scanner";
    
    void CreateScanner(string deskId, string cronExpression);

    void RemoveScanner(string deskId);

    void TriggerScanner(string deskId);

    Task<IGptResponse> TriggerNextStep(int processId);

    Task<IGptResponse> RescheduleNextStep(int processId, DateTime executionTime);

    Task<IGptResponse> StopProcess(int processId);
}