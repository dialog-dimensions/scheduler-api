using Hangfire;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Services.Workflows.Jobs.Classes;
using SchedulerApi.Services.Workflows.Processes.Classes;
using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;
using static SchedulerApi.Models.ChatGPT.Responses.EntityGptResponse;

namespace SchedulerApi.Services.Workflows.Jobs;

public class AutoScheduleProcessJobServices : IAutoScheduleProcessJobServices
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IAutoScheduleProcessRepository _processRepository;

    public AutoScheduleProcessJobServices(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager, IAutoScheduleProcessRepository processRepository)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
        _processRepository = processRepository;
    }

    public void CreateScanner(string deskId, string cronExpression)
    {
        var scannerId = IAutoScheduleProcessJobServices.GetScannerJobId(deskId);
        _recurringJobManager.AddOrUpdate<GptProcessInitiationJob>(
            scannerId,
            job => job.Execute(deskId),
            cronExpression
            );
    }

    public void RemoveScanner(string deskId)
    {
        var scannerId = IAutoScheduleProcessJobServices.GetScannerJobId(deskId);
        _recurringJobManager.RemoveIfExists(scannerId);
    }

    public void TriggerScanner(string deskId)
    {
        var scannerId = IAutoScheduleProcessJobServices.GetScannerJobId(deskId);
        _recurringJobManager.Trigger(scannerId);
    }

    public async Task<IGptResponse> TriggerNextStep(int processId)
    {
        var getProcessResponse = await GetProcessWithScheduledJob(processId);
        if (!getProcessResponse.IsSuccessStatusCode)
        {
            return getProcessResponse;
        }

        var process = (AutoScheduleProcess)getProcessResponse.Content!;
        var jobId = process.NextPhaseJobId;
        
        return _backgroundJobClient.Requeue(jobId) ? Ok() : Problem();
    }

    public async Task<IGptResponse> RescheduleNextStep(int processId, DateTime executionTime)
    {
        var getProcessResponse = await GetProcessWithScheduledJob(processId);
        if (!getProcessResponse.IsSuccessStatusCode)
        {
            return getProcessResponse;
        }

        var process = (AutoScheduleProcess)getProcessResponse.Content!;
        var jobId = process.NextPhaseJobId;
        
        if (_backgroundJobClient.Reschedule(jobId, executionTime.Subtract(DateTime.Now)))
        {
            return Ok();
        }

        return Problem("no job was registered in the process entry in database.");
    }

    public async Task<IGptResponse> StopProcess(int processId)
    {
        var getProcessResponse = await GetProcessWithScheduledJob(processId);
        if (!getProcessResponse.IsSuccessStatusCode)
        {
            return getProcessResponse;
        }

        var process = (AutoScheduleProcess)getProcessResponse.Content!;
        var jobId = process.NextPhaseJobId;
        if (_backgroundJobClient.Delete(jobId))
        {
            return Ok();
        }

        return Problem("Unable to delete job");
    }

    private async Task<IGptResponse> GetProcessWithScheduledJob(int processId)
    {
        var process = await _processRepository.ReadAsync(processId);
        if (process is null)
        {
            return NotFound(typeof(AutoScheduleProcess));
        }

        if (string.IsNullOrEmpty(process.NextPhaseJobId))
        {
            return NotFound(typeof(AutoScheduleProcess));
        }

        return Ok(process);
    }
}
