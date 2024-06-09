using Newtonsoft.Json;
using SchedulerApi.Models.ChatGPT.Requests.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestParser;

public class GptRequestParser : IGptRequestParser
{
    /*
     * POSSIBLE REQUEST TYPES:
     *
     * EMPLOYEE REQUESTS:
     * 1. Read Employee
     * 2. Read Desk Assignment
     * 3. Create Employee
     * 4. Create Desk Assignment
     * 5. Patch Employee
     * 6. Delete Desk Assignment
     * 
     * SCHEDULE REQUESTS:
     * 1. Read Schedule
     * 2. Read Shift
     * 3. Read Exception
     * 4. Get Schedule Exceptions
     * 5. Create Exception
     * 6. Assign Shift
     * 7. Swap Shifts
     * 8. Patch Exception
     * 9. Delete Exception
     *
     * PROCESS REQUESTS:
     * ORGANIZATION REQUESTS:
     */

    public IGptRequest ParseRequest(string requestString)
    {
        var obj = JsonConvert.DeserializeObject<GptRequest>(requestString)!;
        
        // Convert longs to int
        var longs = obj.Parameters
            .Where(kv => kv.Value is long);
        foreach (var (key, value) in longs)
        {
            if (value is not long longValue)
            {
                continue;
            }

            obj.Parameters[key] = Convert.ToInt32(longValue);
        }

        return obj;
    }
}
