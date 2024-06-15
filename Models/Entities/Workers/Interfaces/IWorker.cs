using SchedulerApi.DAL.Queries;

namespace SchedulerApi.Models.Entities.Workers.Interfaces;

public interface IWorker : IMyQueryable
{
    int Id { get; set; }
    string Name { get; set; }
    string Role { get; set; }
}