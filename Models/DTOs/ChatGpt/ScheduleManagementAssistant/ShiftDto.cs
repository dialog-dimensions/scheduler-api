﻿using SchedulerApi.Models.DTOs.Interfaces;
using SchedulerApi.Models.Entities;

namespace SchedulerApi.Models.DTOs.ChatGpt.ScheduleManagementAssistant;

public class ShiftDto : IDto<Shift, ShiftDto>
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime ScheduleStartDateTime { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime ModificationDateTime { get; set; }
    public string ModificationUser { get; set; }
    public string DeskId { get; set; }


    public static ShiftDto FromEntity(Shift entity) => new()
    {
        StartDateTime = entity.StartDateTime,
        EndDateTime = entity.EndDateTime,
        ScheduleStartDateTime = entity.ScheduleStartDateTime,
        EmployeeId = entity.EmployeeId,
        ModificationDateTime = entity.ModificationDateTime,
        ModificationUser = entity.ModificationUser.ToString(),
        DeskId = entity.DeskId
    };

    public Shift ToEntity()
    {
        throw new NotImplementedException();
    }
}