namespace SchedulerApi.Services.Workflows.Jobs.Interfaces;

public interface IGptProcessInitiationJob
{
    Task Execute(string deskId);
}
