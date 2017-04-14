﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models;
using System.Threading;
namespace VoidDays.Services.Interfaces
{
    public interface IAdminService
    {
        bool CheckForCurrentDay(out Day day);
        Day GetCurrentStoredDay();
        Day CreateNextDay(Day currentDay);
        Day GetPreviousDay(Day day);
        Day GetNextDay(Day day);
        List<Day> GetDaysByDayNumber(int start, int end);
        List<Day> GetAllDays();
        Timer SetUpTimer(TimeSpan alertTime);
        Settings GetSettings();
        Day SyncToCurrentDay(Day currentStoredDay);
    }
}