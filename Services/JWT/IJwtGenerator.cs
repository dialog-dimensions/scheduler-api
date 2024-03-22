using Microsoft.AspNetCore.Identity;

namespace SchedulerApi.Services.JWT;

public interface IJwtGenerator
{
    Task<string> Generate(IdentityUser user);
}