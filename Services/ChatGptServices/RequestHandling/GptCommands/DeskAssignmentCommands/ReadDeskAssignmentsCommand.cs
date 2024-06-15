﻿using SchedulerApi.DAL.Queries;
using SchedulerApi.DAL.Repositories.Interfaces;
using SchedulerApi.Models.Organization;
using SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.BaseClasses;

namespace SchedulerApi.Services.ChatGptServices.RequestHandling.GptCommands.DeskAssignmentCommands;

public class ReadDeskAssignmentsCommand : ReadManyCommand<Desk>
{
    public ReadDeskAssignmentsCommand(IQueryService queryService) : base(queryService)
    { }
}
