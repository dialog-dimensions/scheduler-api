using SchedulerApi.DAL.Queries;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Models.Interfaces;
using SchedulerApi.Services.ChatGptServices.Utils;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

public class ReadCommand<T> : IGptCommand where T : class, IKeyProvider
{
    protected IQueryService QueryService { get; set; }

    public ReadCommand(IQueryService queryService)
    {  
        QueryService = queryService;
    }
    
    public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
    {
        (await QueryService.Query(typeof(T), parameters)).ToList().ValidateSingleEntry(out var singleEntryResponse);
        return singleEntryResponse;
    }
}
