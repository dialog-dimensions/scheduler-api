using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.ChatGPT.Responses.Interfaces;
using SchedulerApi.Models.Entities.Workers;
using SchedulerApi.Models.Interfaces;
using static SchedulerApi.Models.ChatGPT.Responses.MessageGptResponse;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

public abstract class CreateCommand<T> : IGptCommand where T : IKeyProvider
{
    protected IRepository<T> Repository { get; set; }

    protected CreateCommand(IRepository<T> repository)
    {
        Repository = repository;
    }

    public async Task<IGptResponse> Execute(Dictionary<string, object> parameters)
    {
        // Validate Parameters
        var validationResponse = await ValidateEntityParameters(parameters);
        if (!validationResponse.IsSuccessStatusCode)
        {
            return validationResponse;
        }
        
        // Create Entity Instance
        var entityParameters = (Dictionary<string, object>)validationResponse.Content!;
        var entityCreated = CreateInstance(entityParameters, out var creationResponse);
        if (!entityCreated)
        {
            return creationResponse;
        }
        
        // Insert Entity Into Database
        var entity = (T)creationResponse.Content!;
        return await InsertToRepositoryAsync(entity);
    }

    public abstract Task<IGptResponse> ValidateEntityParameters(Dictionary<string, object> parameters);
    
    public abstract bool CreateInstance(Dictionary<string, object> parameters, out IGptResponse creationResponse);

    private async Task<IGptResponse> InsertToRepositoryAsync(T entity)
    {
        try
        {
            await Repository.CreateAsync(entity);
        }
        catch (DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
            {
                return Conflict(typeof(T), entity.Key);
            }

            return Problem(
                "A problem occured with the insertion of the entity record to the database. see exception details. " +
                dbUpdateException.Message);
        }
        catch (Exception ex)
        {
            return Problem("An error occured when trying to insert the entity record to the database. " + ex.Message);
        }

        return Created(typeof(Employee), entity.Key);
    }
}
