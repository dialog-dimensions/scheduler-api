using SchedulerApi.DAL.Queries;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using static SchedulerApi.Models.ChatGPT.Responses.EntityGptResponse;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

public class ReadManyCommand<T>: IGptCommand where T : class, IMyQueryable
{
    protected IQueryService QueryService { get; set; }

    protected ReadManyCommand(IQueryService queryService)
    {
        QueryService = queryService;
    }
    
    public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
    {
        var matches = (await QueryService.Query<T>(parameters)).ToList();

        if (!matches.Any())
        {
            return NoContent();
        }

        return Ok(matches);
    }
}
