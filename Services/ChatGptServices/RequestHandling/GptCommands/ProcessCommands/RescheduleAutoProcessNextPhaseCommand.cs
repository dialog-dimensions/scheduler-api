using SchedulerApi.DAL.Queries;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Services.Workflows.Jobs;
using SchedulerApi.Services.Workflows.Processes.Classes;
using static SchedulerApi.Services.ChatGptServices.Utils.ValidationUtils;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ProcessCommands;

public class RescheduleAutoProcessNextPhaseCommand : IGptCommand
{
    private readonly IAutoScheduleProcessJobServices _jobServices;
    private readonly IQueryService _query;

    public RescheduleAutoProcessNextPhaseCommand(IAutoScheduleProcessJobServices jobServices, IQueryService query)
    {
        _jobServices = jobServices;
        _query = query;
    }

    public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
    {
        var validRescheduleDateTime = ValidateParameter<DateTime>(GptRequestType.RescheduleAutoProcessNextPhase, parameters, "ProcessRescheduleDateTime",
            out var rescheduleDateTimeValidationResponse);

        if (!validRescheduleDateTime)
        {
            return rescheduleDateTimeValidationResponse;
        }
        
        var foundSingleProcess = (await _query.Query(typeof(AutoScheduleProcess), parameters))
            .ToList()
            .ValidateSingleEntry(out var processQueryResponse);

        if (!foundSingleProcess)
        {
            return processQueryResponse;
        }

        var process = (AutoScheduleProcess)processQueryResponse.Content!;
        var rescheduleDateTime = (DateTime)rescheduleDateTimeValidationResponse.Content!;
        return await _jobServices.RescheduleNextStep(process.Id, rescheduleDateTime);

    }
}