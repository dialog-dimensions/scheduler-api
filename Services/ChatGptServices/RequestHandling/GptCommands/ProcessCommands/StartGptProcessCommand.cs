using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Enums;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ChatGptServices.Utils;
using SchedulerApi.Services.Workflows.Processes.Interfaces;
using static SchedulerApi.Services.ChatGptServices.Utils.ValidationUtils;
using static SchedulerApi.Models.ChatGPT.Responses.EntityGptResponse;
using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ProcessCommands;

public class StartGptProcessCommand : IGptCommand
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IQueryService _query;

    public StartGptProcessCommand(
        IServiceProvider serviceProvider, IScheduleRepository scheduleRepository, IQueryService query)
    {
        _serviceProvider = serviceProvider;
        _scheduleRepository = scheduleRepository;
        _query = query;
    }

    public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
    {
        var parametersValidationResponse = await ValidateProcessParameters(parameters);
        if (!parametersValidationResponse.IsSuccessStatusCode)
        {
            return parametersValidationResponse;
        }

        var processParameters = (Dictionary<string, object>)parametersValidationResponse.Content!;
        string deskId;
        DateTime startDateTime;
        DateTime endDateTime;
        int shiftDuration;

        try
        {
            deskId = (string)processParameters["DeskId"];
            startDateTime = (DateTime)processParameters["ScheduleStartDateTime"];
            endDateTime = (DateTime)processParameters["ScheduleEndDateTime"];
            shiftDuration = (int)processParameters["ShiftDuration"];
        }
        catch (InvalidCastException ex)
        {
            return Problem("an unhandled cast exception was thrown during parameter extraction. " + ex.Message);
        }
        catch (Exception ex)
        {
            return Problem("an unhandled exception was thrown during parameter extraction. see details. " + ex.Message);
        }

        try
        {
            var process = _serviceProvider.GetRequiredService<IGptScheduleProcess>();
            await process.Run(deskId, startDateTime, endDateTime, shiftDuration);
        }
        catch (Exception ex)
        {
            return Problem("an unhandled exception was thrown during the process initialization. see details. " +
                           ex.Message);
        }

        return Ok();
    }

    private async Task<IGptResponse> ValidateProcessParameters(Dictionary<string, object> parameters)
    {
        // Desk Is Required
        var foundSingleDesk = (await _query.Query<Desk>(parameters))
            .ToList()
            .ValidateSingleEntry(out var deskQueryResponse);

        if (!foundSingleDesk)
        {
            return deskQueryResponse;
        }
        
        var desk = (Desk)deskQueryResponse.Content!;
        
        // Get the latest schedule
        var latestSchedule = await _scheduleRepository.ReadLatestAsync(desk.Id);
        var havePreviousSchedule = latestSchedule is not null;

        // Schedule Start, Schedule End, Shift Duration are optional if have previous schedule.
        // Default is last schedule's parameters

        // Schedule Start
        var validatedScheduleStart = ValidateProcessParameter<DateTime>(
            parameters,
            "ScheduleStartDateTime",
            out var scheduleStartValidationResponse,
            nullable: havePreviousSchedule);

        if (!validatedScheduleStart)
        {
            return scheduleStartValidationResponse;
        }
        
        var scheduleStartDateTime = scheduleStartValidationResponse.StatusCode == "200" // Parameter Given
            ? (DateTime)scheduleStartValidationResponse.Content!
            : latestSchedule!.EndDateTime;
        
        // Schedule End
        var validatedScheduleEnd = ValidateProcessParameter<DateTime>(
            parameters,
            "ScheduleEndDateTime",
            out var scheduleEndValidationResponse,
            nullable: havePreviousSchedule);

        if (!validatedScheduleEnd)
        {
            return scheduleEndValidationResponse;
        }

        var scheduleEndDateTime = scheduleEndValidationResponse.StatusCode == "200" // Parameter Given
            ? (DateTime)scheduleEndValidationResponse.Content!
            : scheduleStartDateTime.Add(latestSchedule!.EndDateTime - latestSchedule.StartDateTime);
        
        // Shift Duration
        var validatedShiftDuration = ValidateProcessParameter<int>(
            parameters,
            "ShiftDuration",
            out var shiftDurationValidationResponse,
            nullable: havePreviousSchedule
        );

        if (!validatedShiftDuration)
        {
            return shiftDurationValidationResponse;
        }

        var shiftDuration = shiftDurationValidationResponse.StatusCode == "200" // Parameter Given
            ? (int)shiftDurationValidationResponse.Content!
            : latestSchedule!.ShiftDuration;

        var processParameters = new Dictionary<string, object>
        {
            { "DeskId", desk.Id },
            { "ScheduleStartDateTime", scheduleStartDateTime },
            { "ScheduleEndDateTime", scheduleEndDateTime },
            { "ShiftDuration", shiftDuration }
        };

        return Ok(processParameters);
    }

    private bool ValidateProcessParameter<T>(Dictionary<string, object> parameters, string paramName,
        out IGptResponse paramValidationResponse, bool nullable = true)
        => ValidateParameter<T>(GptRequestType.StartGptProcess, parameters, paramName, out paramValidationResponse,
            nullable);
}