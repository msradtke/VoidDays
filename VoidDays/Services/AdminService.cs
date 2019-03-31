using System;
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
        IRepositoryBase<Settings> _settingsRepository;
        IUnitOfWork _unitOfWork;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        IEventAggregator _eventAggregator;
        private readonly IDialogService _dialogService;
        private static Settings _settings;
        private Day _currentDay;

        public AdminService(IUnitOfWorkFactory unitOfWorkFactory, IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _unitOfWork = _unitOfWorkFactory.CreateUnitOfWork();
        }
        public void Initialize()
        {
            _settingsRepository = _unitOfWork.SettingsRepository;
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
        public Day CreateToday(IRepositoryBase<Day> dayRepo)
        {//todo: remove _repo
            var day = new Day();
            var settings = GetSettings();
            var firstDay = dayRepo.Get(x => x.DayNumber == 0).FirstOrDefault();
            var daySpan = DateTime.Today - firstDay.Start;
            var dayNum = daySpan.Days;
            day.DayNumber = dayNum + 1;
            day.Start = DateTime.Today + settings.StartTime.TimeOfDay;
            var addDay = day.Start.AddDays(1);
            var subtractSecond = addDay.AddSeconds(-1);
            day.End = subtractSecond;
            day.IsActive = true;
            dayRepo.Insert(day);
            //_unitOfWork.Save();
            //SyncToCurrentDay(currentDay);
            return day;
        }
        public Day CreateNextDay(Day currentDay, IRepositoryBase<Day> dayRepository)
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
            dayRepository.Insert(nextDay);
            dayRepository.Update(currentDay);
            //_unitOfWork.Save();
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
        public Day GetVoidDayFromDayNumber(Day previousDay, int dayNumber)
        {
            var voidDay = new Day();
            var settings = GetSettings();
            voidDay.DayNumber = dayNumber;
            var startDateTime = new DateTime();
            startDateTime = previousDay.Start.Date;
            voidDay.Start = startDateTime.AddDays(dayNumber - previousDay.DayNumber) + settings.StartTime.TimeOfDay;
            var addDay = voidDay.Start.AddDays(1);
            var subtractSecond = addDay.AddSeconds(-1);
            voidDay.End = subtractSecond;
            voidDay.IsActive = false;
            voidDay.IsVoid = true;
            return voidDay;
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
        public List<Day> GetVoidDayRange(int dayStart, int dayEnd)
        {
            if (dayStart > dayEnd)
                throw new InvalidOperationException();

            var allDays = GetAllDays();
            //var highestDayNumber = allDays.Max(x => x.DayNumber);

            List<Day> allVoidDays = new List<Day>();
            Day previousDay = null;
            for (int i = dayStart; i <= dayEnd; ++i)
            {
                var day = allDays.FirstOrDefault(x => x.DayNumber == i);
                if (day == null)
                {
                    if (previousDay == null)
                    {
                        day = GetVoidDayFromDayNumber(allDays.First(), i);
                    }
                    else
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
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var settings = GetSettings();
                if (settings == null)
                {
                    settings = GetDefaultSettings();
                    unitOfWork.SettingsRepository.Insert(settings);
                }
                firstDay.DayNumber = 0;

                firstDay.Start = DateTime.UtcNow.Date + settings.StartTime.TimeOfDay;
                firstDay.End = firstDay.Start.Add(new TimeSpan(23, 59, 59));
                firstDay.IsActive = true;

                unitOfWork.DayRepository.Insert(firstDay);
                unitOfWork.Save();

                //SyncToCurrentDay(currentDay);
            }
            return firstDay;
        }
        public void InsertSettings(Settings settings)
        {
            _settingsRepository.Insert(settings);
        }
        public Settings GetDefaultSettings()
        {
            var settings = new Settings();
            settings.StartTime = new DateTime(2000, 1, 1, 8, 0, 0).ToUniversalTime();
            settings.EndTime = new DateTime(2000, 1, 1, 7, 59, 59).ToUniversalTime();
            settings.StartDay = DateTime.Today.ToUniversalTime();
            return settings;
        }

        public void SetIsLoading(LoadingLock loadLock)
        {

            _eventAggregator.GetEvent<LoadingEvent>().Publish(loadLock);
        }


        public List<GoalItem> GetGoalItemsByDayNumber(int day, IRepositoryBase<GoalItem> goalItemRepo)
        {
            var goalItems = goalItemRepo.Get(x => x.DayNumber == day, null);
            return goalItems.ToList();
        }
        public void SaveGoalItem(GoalItem goalItem, IRepositoryBase<Goal> goalRepo, IRepositoryBase<GoalItem> goalItemRepo)
        {
            goalItemRepo.Update(goalItem);
            goalItem.Goal.Message = goalItem.Message;
            goalItem.Goal.Title = goalItem.Title;
            goalRepo.Update(goalItem.Goal);
        }

        public Day SyncToCurrentDay(Day currentStoredDay)
        {
            try
            {
                Day current;
                using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    var goalItemRepo = unitOfWork.GoalItemRepository;
                    var goalRepo = unitOfWork.GoalRepository;
                    if (!CheckForCurrentDay(currentStoredDay, out current)) //if latest day in db is not today
                    {
                        var dayRepository = unitOfWork.DayRepository;
                        currentStoredDay = dayRepository.Get(x => x.DayId == current.DayId).FirstOrDefault();
                        //complete this day, foreach goalitem on this day not completed, void
                        var goalItems = GetGoalItemsByDayNumber(currentStoredDay.DayNumber, goalItemRepo).ToList();
                        bool noGoalItems = true;
                        foreach (var item in goalItems)
                        {
                            noGoalItems = false;
                            if (item.IsComplete == false)
                            {
                                item.IsVoid = true;
                                currentStoredDay.IsVoid = true;
                            }
                            SaveGoalItem(item, goalRepo, goalItemRepo);
                        }
                        if (noGoalItems)
                            currentStoredDay.IsVoid = true;
                        currentStoredDay.IsActive = false;
                        var nextDay = CreateToday(dayRepository);
                        CreateAllGoalItems(nextDay.DayNumber, unitOfWork.GoalRepository, unitOfWork.GoalItemRepository);
                        try
                        {
                            unitOfWork.Save();
                            //_dialogService.OpenErrorDialog("save success", "success");
                        }
                        catch
                        {
                            Log.DebugLog("Day already created");
                            return null;
                        }
                        return nextDay;
                    }
                }
                return current;
            }
            catch (Exception e)
            {
                _dialogService.OpenErrorDialog(e.Message, "sync to current day");
                return null;
            }
        }
        public void CreateAllGoalItems(int dayNumber, IRepositoryBase<Goal> goalRepo, IRepositoryBase<GoalItem> goalItemRepo)
        {
            if (goalRepo == null || goalItemRepo == null)
                return;

            var goals = goalRepo.Get(x => x.IsActive).ToList();
            foreach (var goal in goals)
            {
                var goalitem = new GoalItem();
                goalitem.Goal = goal;
                goalitem.IsComplete = false;
                goalitem.Message = goal.Message;
                goalitem.Title = goal.Title;
                goalitem.DayNumber = dayNumber;
                goalitem.DateTime = DateTime.UtcNow;
                goalItemRepo.Insert(goalitem);
            }
        }
        public List<Day> GetDaysByDayNumber(int start, int end)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var days = unitOfWork.DayRepository.Get(x => x.DayNumber >= start && x.DayNumber <= end).ToList();
                return days;
            }
        }

        public Day GetCurrentStoredDay()
        {
            Day currentStoredDay;
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                currentStoredDay = unitOfWork.DayRepository.Get().LastOrDefault();
            }
            if (currentStoredDay == null)
                Console.WriteLine("current stored day is null");
            return currentStoredDay;

        }
        public List<Day> GetAllDays()
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return unitOfWork.DayRepository.Get().ToList();
            }
        }
        public Settings GetSettings(bool reload = false)
        {
            if (_settings == null || reload == true)
                using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    _settings = unitOfWork.SettingsRepository.Get().FirstOrDefault();
                }
            return _settings;
        }
        private Timer _timer;

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

            //var t = new System.Timers.Timer();
            //this.timer = new Timer(timeToGo.Milliseconds);
            _timer = new Timer(10000);
            _timer.AutoReset = true;
            _timer.Elapsed += NextDayHandler;
            _timer.Enabled = true;

            return _timer;
        }
        public Timer SetUpTimer(TimeSpan alertTime)
        {
            return SetupTimer(GetCurrentStoredDay(), alertTime);
        }
        public void DisableTimer()
        {
            _timer.Enabled = false;
        }
        private void QueueCheckForUpdatedDay()
        {
            Day currentStoredDay = GetCurrentStoredDay();
        }
        private void NextDayHandler(object o, ElapsedEventArgs e)
        {
            
            var timer = (Timer)o;
            _timer.Interval = 10000;
            //timer.Enabled = false;
            DateTime current = DateTime.UtcNow;
            Day currentStoredDay = GetCurrentStoredDay();
            _settings = GetSettings();
            //if (current.Date > _currentDay.Start.Date && current.TimeOfDay > _settings.EndTime)
            if (current.Date > currentStoredDay.Start.Date && current.TimeOfDay > _settings.EndTime.TimeOfDay)
            {
                _timer.Interval = 60000;
                using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
                {

                    var loadLock = new LoadingLock { Id = Guid.NewGuid(), IsLoading = true };
                    SetIsLoading(loadLock);
                    Log.GeneralLog("Creating Next Day");
                    //check if other client already next dayed
                    //day in db

                    Day updatedDay = SyncToCurrentDay(currentStoredDay); //day is the new updated day
                    if (updatedDay == null)
                    {
                        //timer.Enabled = true;
                        loadLock.IsLoading = false;
                        SetIsLoading(loadLock);
                        return;
                    }

                    if (updatedDay != null) //actually updated the day, null if no update
                    {
                        _currentDay = updatedDay;
                        _eventAggregator.GetEvent<NextDayEvent>().Publish(updatedDay);
                        Log.GeneralLog("Published next day event");
                    }
                    //timer = SetupTimer(_currentDay, _settings.EndTime);
                    //Log.GeneralLog(String.Format("setup timer, current day = {0}", _currentDay.DayNumber));
                    //Log.GeneralLog(String.Format("setup timer, EndTime = {0}", _settings.EndTime.ToString()));

                    //timer.Enabled = true;
                    loadLock.IsLoading = false;
                    SetIsLoading(loadLock);
                }
            }
            else
            {
                _eventAggregator.GetEvent<CheckNextDayEvent>().Publish(currentStoredDay);
                //timer.Enabled = true;
            }
        }
        void TestNextDayHandler(object o, ElapsedEventArgs e)
        {
            //_unitOfWork = null;
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var dayRepo = unitOfWork.DayRepository;
                var goalRepo = unitOfWork.GoalRepository;
                var goalItemRepo = unitOfWork.GoalItemRepository;
                _currentDay = dayRepo.Get().LastOrDefault();
                var goalItems = GetGoalItemsByDayNumber(_currentDay.DayNumber, goalItemRepo).ToList();
                bool noGoalItems = true;
                foreach (var item in goalItems)
                {
                    noGoalItems = false;
                    if (item.IsComplete == false)
                    {
                        item.IsVoid = true;
                        _currentDay.IsVoid = true;
                    }
                    SaveGoalItem(item, goalRepo, goalItemRepo);
                }
                if (noGoalItems)
                    _currentDay.IsVoid = true;
                _currentDay.IsActive = false;

                var day = new Day();
                var settings = GetSettings();
                var firstDay = dayRepo.Get(x => x.DayNumber == 0).FirstOrDefault();
                var daySpan = DateTime.Today - firstDay.Start;
                var dayNum = daySpan.Days;
                day.DayNumber = _currentDay.DayNumber + 1;
                day.Start = _currentDay.Start.AddDays(1);
                var addDay = day.Start.AddDays(1);
                var subtractSecond = addDay.AddSeconds(-1);
                day.End = subtractSecond;
                day.IsActive = true;
                dayRepo.Insert(day);
                //SyncToCurrentDay(currentDay);
                CreateAllGoalItems(day.DayNumber, goalRepo, goalItemRepo);
                _currentDay = day;
                unitOfWork.Save();
                _eventAggregator.GetEvent<NextDayEvent>().Publish(_currentDay);
            }
        }
    }
}

