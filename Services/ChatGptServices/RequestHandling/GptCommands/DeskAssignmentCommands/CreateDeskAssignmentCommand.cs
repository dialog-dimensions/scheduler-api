using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ChatGptServices.Utils;
using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.DeskAssignmentCommands;

public class CreateDeskAssignmentCommand : IGptCommand
{
    private readonly IDeskRepository _deskRepository;
    private readonly IQueryService _queryService;

    public CreateDeskAssignmentCommand(
        IDeskRepository deskRepository,
        IQueryService queryService)
    {
        _deskRepository = deskRepository;
        _queryService = queryService;
    }

    public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
    {
        try
        {
            // Get Desk
            var foundSingleDesk = (await _queryService.Query(typeof(Desk), parameters))
                .ToList()
                .ValidateSingleEntry(out var deskQueryResponse);

            // If not Exactly One Desk Found
            if (!foundSingleDesk)
            {
                return deskQueryResponse;
            }

            // Get Employee
            var foundSingleEmployee = (await _queryService.Query(typeof(Employee), parameters))
                .ToList()
                .ValidateSingleEntry(out var employeeQueryResponse);

            // If not Exactly One Employee Found 
            if (!foundSingleEmployee)
            {
                return employeeQueryResponse;
            }

            // Pull the Required Entities Out of the Response Contents
            var desk = (Desk)deskQueryResponse.Content!;
            var employee = (Employee)employeeQueryResponse.Content!;

            // Execute the Command.
            await _deskRepository.AddDeskAssignment(desk, employee);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}