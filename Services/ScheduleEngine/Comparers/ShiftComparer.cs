using SchedulerApi.Models.Entities;
using SchedulerApi.Models.Entities.Enums;
using SchedulerApi.Models.ScheduleEngine;
using SchedulerApi.Services.ScheduleEngine.Comparers.Interfaces;

namespace SchedulerApi.Services.ScheduleEngine.Comparers;

public class ShiftComparer : IShiftComparer
{
    private ScheduleData? Data { get; set; }
    private bool Initialized => Data is not null;
    public bool Ready => Initialized;
    

    public void Initialize(ScheduleData data)
    {
        SetContext(data);
    }
    
    private void SetContext(ScheduleData data)
    {
        Data = data;
    }

    public int Compare(Shift? x, Shift? y)
    {
        if (!Ready) return 0;
        
        if (x is null)
        {
            return y is null ? 0 : 1;
        }

        return y is null ? -1 : CompareNotNull(x, y);
    }

    private int CompareNotNull(Shift x, Shift y)
    {
        if (x.IsDifficult & !y.IsDifficult) return -1;
        if (!x.IsDifficult & y.IsDifficult) return 1;

        var countConstraintsX = CountExceptions(x, ExceptionType.Constraint);
        var countConstraintsY = CountExceptions(y, ExceptionType.Constraint);

        if (countConstraintsX > countConstraintsY)
        {
            return -1;
        }
        if (countConstraintsY > countConstraintsX)
        {
            return 1;
        }

        var countOffPreferencesX = CountExceptions(x, ExceptionType.OffPreference);
        var countOffPreferencesY = CountExceptions(y, ExceptionType.OffPreference);

        if (countOffPreferencesX > countOffPreferencesY)
        {
            return -1;
        }
        if (countOffPreferencesY > countOffPreferencesX)
        {
            return 1;
        }

        var countOnPreferencesX = CountExceptions(x, ExceptionType.OnPreference);
        var countOnPreferencesY = CountExceptions(y, ExceptionType.OnPreference);

        if (countOnPreferencesX > 0 & countOnPreferencesY == 0)
        {
            return -1;
        }
        if (countOnPreferencesY > 0 & countOnPreferencesX == 0)
        {
            return 1;
        }
        if (countOnPreferencesY > countOnPreferencesX)
        {
            return -1;
        }

        if (countOnPreferencesX > countOnPreferencesY)
        {
            return 1;
        }

        return x.StartDateTime < y.StartDateTime ? -1 : 1;
    }

    private int CountExceptions(Shift shift, ExceptionType exceptionType)
    {
        return Data!.Exceptions.Count(ex => 
            ex.ShiftKey.Equals(shift.StartDateTime) & 
            ex.ExceptionType == exceptionType);
    }
}