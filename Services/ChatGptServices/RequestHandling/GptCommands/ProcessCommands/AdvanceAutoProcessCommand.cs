using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Services.ChatGptServices.Utils;
using SchedulerApi.Services.Workflows.Jobs;
using SchedulerApi.Services.Workflows.Processes.Classes;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ProcessCommands;

public class AdvanceAutoProcessCommand : IGptCommand
{
    private readonly IAutoScheduleProcessJobServices _jobServices;
    private readonly IQueryService _queryService;

    public AdvanceAutoProcessCommand(IAutoScheduleProcessJobServices jobServices, IQueryService queryService)
    {
        _jobServices = jobServices;
        _queryService = queryService;
    }

    public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
    {
        var foundSingleProcess = (await _queryService.Query(typeof(AutoScheduleProcess), parameters)).ToList()
            .ValidateSingleEntry(out var processQueryResponse);
        
        if (!foundSingleProcess)
        {
            return processQueryResponse;
        }

        var process = (AutoScheduleProcess)processQueryResponse.Content!;
        return await _jobServices.TriggerNextStep(process.Id);
    }
}