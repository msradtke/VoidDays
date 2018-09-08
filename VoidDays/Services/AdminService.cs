﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Services.Interfaces;
using VoidDays.Models.Interfaces;
using VoidDays.Models;
using System.Timers;
using VoidDays.ViewModels.Events;
using Prism.Events;
using VoidDays.Logging;
using VoidDays.Debug;

namespace VoidDays.Services
{
    public class AdminService : IAdminService
    {
        IRepositoryBase<Day> _dayRepository;
        IRepositoryBase<GoalItemsCreated> _goalItemsCreatedRepository;
        IRepositoryBase<Settings> _settingsRepository;
        IRepositoryBase<GoalItem> _goalItemRepository;
        IRepositoryBase<Goal> _goalRepository;
        IUnitOfWork _unitOfWork;
        IEventAggregator _eventAggregator;
        Settings _settings;
        private Day _currentDay;
        private bool _checkForDbUpdate; //for checking if other client updated db
        public AdminService(IUnitOfWork unitOfWork, IEventAggregator eventAggregator)
        {
            _checkForDbUpdate = false;
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            _dayRepository = _unitOfWork.DayRepository;
        }
        public void Initialize()
        {
            _goalItemsCreatedRepository = _unitOfWork.GoalItemsCreatedRepository;
            _goalItemRepository = _unitOfWork.GoalItemRepository;
            _settingsRepository = _unitOfWork.SettingsRepository;
            _goalRepository = _unitOfWork.GoalRepository;
            _settings = GetSettings();

            //debug
            if (_settings == null)
                DebugService.InitSettings();
        }
        public bool CheckForCurrentDay(Day currentStoredDay, out Day day)
        {
            day = currentStoredDay;
            if (currentStoredDay == null)
            {
                day = CreateFirstDay();
                return true;
            }
            if (DateTime.UtcNow < currentStoredDay.End) //if todays date is equal to current stored date
            {
                return true;
            }
            // currently stored date is not in sync
            return false;
        }
        public bool CheckForCurrentDay(out Day day)
        {
            Day day2 = null;
            var isCurrentDay = CheckForCurrentDay(GetCurrentStoredDay(), out day2);
            day = day2;
            return isCurrentDay;
        }
        public Day CreateToday()
        {
            var day = new Day();
            var settings = GetSettings();
            var firstDay = _dayRepository.Get(x => x.DayNumber == 0).FirstOrDefault();
            var daySpan = DateTime.Today - firstDay.Start;
            var dayNum = daySpan.Days;
            day.DayNumber = dayNum + 1;
            day.Start = DateTime.Today + settings.StartTime.TimeOfDay;
            var addDay = day.Start.AddDays(1);
            var subtractSecond = day.Start.AddSeconds(-1);
            day.End = subtractSecond;
            day.IsActive = true;
            _dayRepository.Insert(day);
            _unitOfWork.Save();
            //SyncToCurrentDay(currentDay);
            return day;
        }
        public Day CreateNextDay(Day currentDay)
        {
            currentDay.IsActive = false;
            var nextDay = new Day();
            var settings = GetSettings();
            nextDay.DayNumber = currentDay.DayNumber + 1;
            var startDateTime = new DateTime();
            startDateTime = currentDay.Start.Date;
            nextDay.Start = startDateTime.AddDays(1) + settings.StartTime.TimeOfDay;
            var addDay = nextDay.Start.AddDays(1);
            var subtractSecond = addDay.AddSeconds(-1);
            nextDay.End = subtractSecond;
            nextDay.IsActive = true;
            _dayRepository.Insert(nextDay);
            _dayRepository.Update(currentDay);
            _unitOfWork.Save();
            //SyncToCurrentDay(currentDay);
            return nextDay;
        }
        public Day GetVoidDayAfter(Day previousday)
        {
            previousday.IsActive = false;
            var nextDay = new Day();
            var settings = GetSettings();
            nextDay.DayNumber = previousday.DayNumber + 1;
            var startDateTime = new DateTime();
            startDateTime = previousday.Start.Date;
            nextDay.Start = startDateTime.AddDays(1) + settings.StartTime.TimeOfDay;
            var addDay = nextDay.Start.AddDays(1);
            var subtractSecond = addDay.AddSeconds(-1);
            nextDay.End = subtractSecond;
            nextDay.IsActive = false;
            nextDay.IsVoid = true;
            return nextDay;
        }
        public List<Day> GetAllVoidDays()
        {
            var allDays = GetAllDays();
            var highestDayNumber = allDays.Max(x => x.DayNumber);

            List<Day> allVoidDays = new List<Day>();
            Day previousDay = null;
            for (int i = 0; i <= highestDayNumber; ++i)
            {
                var day = allDays.FirstOrDefault(x => x.DayNumber == i);
                if (day == null)
                {
                    if (previousDay == null)
                        throw new InvalidOperationException();
                    day = GetVoidDayAfter(previousDay);
                }

                allVoidDays.Add(day);
                previousDay = day;
            }
            return allVoidDays;
        }
        public Day CreateFirstDay()
        {
            var firstDay = new Day();
            var settings = GetSettings();
            if (settings == null)
            {
                settings = GetDefaultSettings();
                InsertSettings(settings);
            }
            firstDay.DayNumber = 0;

            firstDay.Start = DateTime.UtcNow.Date + settings.StartTime.TimeOfDay;
            firstDay.End = firstDay.Start.Add(new TimeSpan(23, 59, 59));
            firstDay.IsActive = true;

            _dayRepository.Insert(firstDay);
            _unitOfWork.Save();

            //SyncToCurrentDay(currentDay);
            return firstDay;
        }
        public void InsertSettings(Settings settings)
        {
            _settingsRepository.Insert(settings);
        }
        public Settings GetDefaultSettings()
        {
            var settings = new Settings();
            
            settings.StartDay = DateTime.Today.ToUniversalTime();
            settings.StartTime = new DateTime(2000, 1, 1, 8, 0, 0);
            settings.EndTime = new DateTime(2000, 1, 1, 7, 59, 59);
            settings.IsUpdating = false;
            return settings;
        }

        public void SetIsLoading(LoadingLock loadLock)
        {

            _eventAggregator.GetEvent<LoadingEvent>().Publish(loadLock);
        }


        public List<GoalItem> GetGoalItemsByDayNumber(int day)
        {
            var goalItems = _goalItemRepository.Get(x => x.DayNumber == day, null);
            return goalItems.ToList();
        }
        public void SaveGoalItem(GoalItem goalItem)
        {
            _goalItemRepository.Update(goalItem);
            goalItem.Goal.Message = goalItem.Message;
            goalItem.Goal.Title = goalItem.Title;
            _goalRepository.Update(goalItem.Goal);
        }

        public Day SyncToCurrentDay(Day currentStoredDay)
        {
            Day current;
            if (!CheckForCurrentDay(currentStoredDay, out current)) //if latest day in db is not today
            {
                //complete this day, foreach goalitem on this day not completed, void
                var goalItems = GetGoalItemsByDayNumber(currentStoredDay.DayNumber).ToList();
                bool noGoalItems = true;
                foreach (var item in goalItems)
                {
                    noGoalItems = false;
                    if (item.IsComplete == false)
                    {
                        item.IsVoid = true;
                        currentStoredDay.IsVoid = true;
                    }
                    SaveGoalItem(item);
                }
                if (noGoalItems)
                    currentStoredDay.IsVoid = true;
                currentStoredDay.IsActive = false;
                SaveChanges();
                var nextDay = CreateToday(); 

                CreateAllGoalItems(nextDay.DayNumber);
                
                return nextDay;
            }
            return current;
        }

        public void SaveChanges()
        {
            _unitOfWork.Save();
        }
        public void CreateAllGoalItems(int dayNumber)
        {
            var goals = _goalRepository.Get(x => x.IsActive).ToList();
            foreach (var goal in goals)
            {
                var goalitem = new GoalItem();
                goalitem.Goal = goal;
                goalitem.IsComplete = false;
                goalitem.Message = goal.Message;
                goalitem.Title = goal.Title;
                goalitem.DayNumber = dayNumber;
                goalitem.DateTime = DateTime.UtcNow;
                _goalItemRepository.Insert(goalitem);
            }
        }
        public List<Day> GetDaysByDayNumber(int start, int end)
        {
            var days = _dayRepository.Get(x => x.DayNumber >= start && x.DayNumber <= end).ToList();
            foreach (var day in days)
                _unitOfWork.Reload(day);
            return days;
        }
        public Day GetCurrentStoredDay()
        {
            var currentStoredDay = _dayRepository.Get().LastOrDefault();
            _unitOfWork.Reload(currentStoredDay);
            return currentStoredDay;
        }
        public Day GetPreviousDay(Day day)
        {
            return _dayRepository.Get(x => x.DayNumber == day.DayNumber - 1).LastOrDefault();
        }
        public GoalItemsCreated GetLatestGoalItemsCreated()
        {
            return _goalItemsCreatedRepository.Get().LastOrDefault();
        }
        public List<Day> GetAllDays()
        {
            return _dayRepository.Get().ToList();
        }
        public Settings GetSettings()
        {
            var settings = _settingsRepository.Get().FirstOrDefault();
            _unitOfWork.Reload(settings);
            return settings;
        }
        public Day GetNextDay(Day day)
        {
            return _dayRepository.Get(x => x.DayNumber == day.DayNumber + 1).LastOrDefault();
        }
        private Timer timer; 

        public Timer SetupTimer(Day currentDay, TimeSpan alertTime)
        {
            Log.GeneralLog(String.Format("SetupTimer, current day = {0}", currentDay.DayNumber));
            Log.GeneralLog(String.Format("SetupTimer, alertTime = {0}", alertTime.ToString()));

            _currentDay = currentDay;
            DateTime current = DateTime.UtcNow;


            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            //if (timeToGo < TimeSpan.Zero)
            //{
            //SetUpTimer(alertTime.Add(new TimeSpan(24, 0, 0)));
            //return timer;//time already passed
            //}

            var t = new System.Timers.Timer();
            //this.timer = new Timer(timeToGo.Milliseconds);
            this.timer = new Timer(5000);
            this.timer.AutoReset = true;
            timer.Elapsed += NextDayHandler;
            timer.Enabled = true;

            return timer;
        }
        public Timer SetUpTimer(TimeSpan alertTime)
        {
            return SetupTimer(GetCurrentStoredDay(), alertTime);
        }
        private void QueueCheckForUpdatedDay()
        {
            Day currentStoredDay = GetCurrentStoredDay();
        }
        private void NextDayHandler(object o, ElapsedEventArgs e)
        {
            var timer = (Timer)o;
            DateTime current = DateTime.UtcNow;
            Day currentStoredDay = GetCurrentStoredDay();

            //if (current.Date > _currentDay.Start.Date && current.TimeOfDay > _settings.EndTime)
            if (current.Date > currentStoredDay.Start.Date && current.TimeOfDay > _settings.EndTime.TimeOfDay)
            {
                timer.Enabled = false;
                var loadLock = new LoadingLock { Id = Guid.NewGuid(), IsLoading = true };
                SetIsLoading(loadLock);
                Log.GeneralLog("NextDayHandler");
                //check if other client already next dayed
                //day in db
                Day updatedDay = SyncToCurrentDay(currentStoredDay); //day is the new updated day
                SaveChanges();

                if (updatedDay != null) //actually updated the day, null if no update
                {
                    _currentDay = updatedDay;
                    _eventAggregator.GetEvent<NextDayEvent>().Publish(updatedDay);
                    Log.GeneralLog("Published next day event");
                }
                //timer = SetupTimer(_currentDay, _settings.EndTime);
                Log.GeneralLog(String.Format("setup timer, current day = {0}", _currentDay.DayNumber));
                Log.GeneralLog(String.Format("setup timer, EndTime = {0}", _settings.EndTime.ToString()));

                timer.Enabled = true;
                loadLock.IsLoading = false;
                SetIsLoading(loadLock);
            }
        }
    }
}

