using SchedulerApi.Models.DTOs.Interfaces;

namespace SchedulerApi.Models.DTOs;

public class CallToRegisterDto : IDto<CallToRegisterDto, CallToRegisterDto>
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; }

    public static CallToRegisterDto FromEntity(CallToRegisterDto entity) => entity;
    public CallToRegisterDto ToEntity() => this;
}
