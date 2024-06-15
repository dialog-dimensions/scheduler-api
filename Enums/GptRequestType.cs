namespace SchedulerApi.Enums;

public enum GptRequestType
{
    CreateEmployee,
    ReadEmployee,
    ReadEmployees,
    ReadShift,
    ReadShifts,
    AssignShiftEmployee,
    ReadSchedule,
    ReadSchedules,
    CreateShiftException,
    ReadShiftException,
    ReadShiftExceptions,
    ReadDesk,
    ReadDesks,
    CreateDeskAssignment,
    ReadDeskAssignment,
    ReadDeskAssignments,
    DeleteDeskAssignment,
    ReadUnit,
    ReadUnits,
    ReadAutoProcess,
    AdvanceAutoProcess,
    StartGptProcess,
    PauseAutoProcess,
    RescheduleAutoProcessNextPhase
    // SwapShiftEmployees 
    // ReadAutoProcesses
}
