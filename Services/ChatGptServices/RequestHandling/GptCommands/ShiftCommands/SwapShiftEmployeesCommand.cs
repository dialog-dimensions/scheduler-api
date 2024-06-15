// using SchedulerApi.DAL.Queries;
// using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
// using SchedulerApi.Models.Entities;
// using SchedulerApi.Services.ChatGptServices.Utils;
// using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;
//
// namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ShiftCommands;
//
// public class SwapShiftEmployeesCommand : IGptCommand
// {
//     private readonly IQueryService _query;
//
//     public SwapShiftEmployeesCommand(IQueryService query)
//     {
//         
//         _query = query;
//     }
//
//     public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
//     {
//         var foundSingleFirstShift = (await _query.Query<Shift>(parameters))
//             .ToList()
//             .ValidateSingleEntry(out var firstShiftQueryResponse);
//
//         if (!foundSingleFirstShift)
//         {
//             return firstShiftQueryResponse;
//         }
//
//         var firstShift = (Shift)firstShiftQueryResponse.Content!;
//
//         if (firstShift.Employee is null)
//         {
//             return Problem("First shift is not assigned an employee.");
//         }
//         
//         var foundSingleSecondShift = (await _query.Query<Shift>(parameters))
//             .ToList()
//             .ValidateSingleEntry(out var secondShiftQueryResponse);
//
//         if (!foundSingleSecondShift)
//         {
//             return secondShiftQueryResponse;
//         }
//
//         var secondShift = (Shift)secondShiftQueryResponse.Content!;
//
//         if (secondShift.Employee is null)
//         {
//             return Problem("Second shift is not assigned an employee.");
//         }
//
//         if (
//             firstShift.ScheduleStartDateTime != secondShift.ScheduleStartDateTime ||
//             firstShift.DeskId != secondShift.DeskId
//             )
//         {
//             return Problem("Cannot swap between shifts of different schedules.");
//         }
//
//         if (firstShift.ScheduleStartDateTime < DateTime.Now)
//         {
//             return Problem("Cannot edit shifts of past or present schedules.");
//         }
//         
//         var firstEmployee = firstShift.Employee;
//         firstShift.Employee = secondShift.Employee;
//         secondShift.Employee = firstEmployee;
//         return Ok();
//     }
//     
//     
// }
// TODO: currently unable to distinct first and second shift.
