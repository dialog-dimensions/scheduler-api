using Hangfire.Dashboard;

namespace SchedulerApi.Services.JWT.AuthorizationFilters;

public class JwtAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
