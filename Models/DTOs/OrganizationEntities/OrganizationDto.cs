using SchedulerApi.Models.DTOs.Interfaces;

namespace SchedulerApi.Models.DTOs.OrganizationEntities;

public class OrganizationDto : IDto<Organization.Organization, OrganizationDto>
{
    
    
    public static OrganizationDto FromEntity(Organization.Organization entity)
    {
        throw new NotImplementedException();
    }

    public Organization.Organization ToEntity()
    {
        throw new NotImplementedException();
    }
}