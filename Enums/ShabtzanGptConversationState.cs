namespace SchedulerApi.Enums;

public enum ShabtzanGptConversationState
{
    NotCreated,
    Created,
    Initialization,
    AwaitingGathering,
    Gathering,
    JsonTransmission,
    JsonDetected,
    Ended,
    Faulted
}
