using SchedulerApi.Models.Interfaces;

namespace SchedulerApi.Models.Entities.Workers.Interfaces;

public interface IWorker : IKeyProvider, IEquatable<IWorker>
{
    int Id { get; set; }
    string Name { get; set; }
    string Role { get; set; }
}