using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models;
using System.Timers;
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
        Timer SetupTimer(Day currentDay, TimeSpan alertTime);
        Timer SetUpTimer(TimeSpan alertTime);
        Settings GetSettings(bool reload = false);
        Day SyncToCurrentDay(Day currentStoredDay);
        void Initialize();
        void SetIsLoading(LoadingLock loadLock);
        Day GetVoidDayAfter(Day previousDay);
        List<Day> GetAllVoidDays();
        List<Day> GetVoidDayRange(int dayStart, int dayEnd);
        void DisableTimer();
    }
}
