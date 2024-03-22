using Microsoft.AspNetCore.Identity;
using SchedulerApi.Models.DTOs.Interfaces;

namespace SchedulerApi.Models.DTOs;

public class UserDto : IDto<IdentityUser, UserDto>
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }

    public static UserDto FromEntity(IdentityUser entity) => new()
    {
        Id = entity.Id,
        UserName = entity.UserName!,
        PhoneNumber = entity.PhoneNumber!
    };

    public IdentityUser ToEntity()
    {
        throw new NotImplementedException();
    }
}
