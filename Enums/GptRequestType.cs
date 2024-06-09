namespace SchedulerApi.Enums;

public enum GptRequestType
{
    CreateEmployee,
    CreateDeskAssignment,
    CreateShiftException,
    ReadEmployee,
    ReadDeskAssignment,
    ReadSchedule,
    ReadShift,
    ReadShiftException,
    GetScheduleShiftExceptions,
    PatchEmployee,
    AssignShift,
    SwapShifts,
    PatchShiftException,
    DeleteDeskAssignment,
    DeleteShiftException
}