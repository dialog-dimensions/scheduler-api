using SchedulerApi.Enums;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.DeskAssignmentCommands;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.DeskCommands;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.EmployeeCommands;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ProcessCommands;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ScheduleCommands;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ShiftCommands;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.ShiftExceptionCommands;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.UnitCommands;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.CommandRegistry;

public class CommandRegistry : ICommandRegistry
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<GptRequestType, Type> _commands;

    public CommandRegistry(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _commands = new Dictionary<GptRequestType, Type>
        {
            { GptRequestType.CreateEmployee, typeof(CreateEmployeeCommand) },
            { GptRequestType.ReadEmployee, typeof(ReadEmployeeCommand) },
            { GptRequestType.ReadEmployees, typeof(ReadEmployeesCommand) },
            { GptRequestType.ReadShift, typeof(ReadShiftCommand) },
            { GptRequestType.ReadShifts, typeof(ReadShiftsCommand) },
            { GptRequestType.AssignShiftEmployee, typeof(AssignShiftEmployeeCommand) },
            // { GptRequestType.SwapShiftEmployees, typeof(SwapShiftEmployeesCommand) },
            { GptRequestType.ReadSchedule, typeof(ReadScheduleCommand) },
            { GptRequestType.ReadSchedules, typeof(ReadSchedulesCommand) },
            { GptRequestType.CreateShiftException, typeof(CreateShiftExceptionCommand) },
            { GptRequestType.ReadShiftException, typeof(ReadShiftExceptionCommand) },
            { GptRequestType.ReadShiftExceptions, typeof(ReadShiftExceptionsCommand) },
            { GptRequestType.ReadDesk, typeof(ReadDeskCommand) },
            { GptRequestType.ReadDesks, typeof(ReadDesksCommand) },
            { GptRequestType.CreateDeskAssignment, typeof(CreateDeskAssignmentCommand) },
            { GptRequestType.ReadDeskAssignment, typeof(ReadDeskAssignmentCommand) },
            { GptRequestType.ReadDeskAssignments, typeof(ReadDeskAssignmentsCommand) },
            { GptRequestType.DeleteDeskAssignment, typeof(DeleteDeskAssignmentCommand) },
            { GptRequestType.ReadUnit, typeof(ReadUnitCommand) },
            { GptRequestType.ReadUnits, typeof(ReadUnitsCommand) },
            { GptRequestType.ReadAutoProcess, typeof(ReadAutoProcessCommand) },
            // { GptRequestType.ReadAutoProcesses, typeof(ReadAutoProcessesCommand) },
            { GptRequestType.AdvanceAutoProcess, typeof(AdvanceAutoProcessCommand) },
            { GptRequestType.StartGptProcess, typeof(StartGptProcessCommand) },
            { GptRequestType.PauseAutoProcess, typeof(PauseAutoProcessCommand) },
            { GptRequestType.RescheduleAutoProcessNextPhase, typeof(RescheduleAutoProcessNextPhaseCommand) }
        };

    }

    public IGptCommand GetCommand(GptRequestType requestType)
    {
        if (_commands.TryGetValue(requestType, out var commandType))
        {
            return (IGptCommand)_serviceProvider.GetRequiredService(commandType);
        }

        throw new ArgumentException("No command registered for this request type.");
    }
}
