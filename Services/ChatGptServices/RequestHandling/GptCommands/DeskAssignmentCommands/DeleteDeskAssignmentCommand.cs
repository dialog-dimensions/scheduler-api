using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ChatGptServices.Utils;
using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.DeskAssignmentCommands;

public class DeleteDeskAssignmentCommand : IGptCommand
{
    private readonly IDeskRepository _deskRepository;
    private readonly IQueryService _queryService;

    public DeleteDeskAssignmentCommand(IDeskRepository deskRepository, IQueryService queryService)
    {
        _deskRepository = deskRepository;
        _queryService = queryService;
    }

    public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
    {
        var foundSingleDesk = (await _queryService.Query(typeof(Desk), parameters))
            .ToList()
            .ValidateSingleEntry(out var deskQueryResponse);

        if (!foundSingleDesk)
        {
            return deskQueryResponse;
        }

        var foundSingleEmployee = (await _queryService.Query(typeof(Employee), parameters))
            .ToList()
            .ValidateSingleEntry(out var employeeQueryResponse);

        if (!foundSingleEmployee)
        {
            return employeeQueryResponse;
        }

        var desk = (Desk)deskQueryResponse.Content!;
        var employee = (Employee)employeeQueryResponse.Content!;

        try
        {
            await _deskRepository.RemoveDeskAssignment(desk, employee);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }

        return Ok();
    }
}
