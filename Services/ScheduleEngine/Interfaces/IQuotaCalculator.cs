using SchedulerApi.Models.ScheduleEngine;

namespace SchedulerApi.Services.ScheduleEngine.Interfaces;

public interface IQuotaCalculator
{
    ScheduleQuotas GetQuotas(ScheduleData data);
}