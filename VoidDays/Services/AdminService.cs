﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Services.Interfaces;
using VoidDays.Models.Interfaces;
using VoidDays.Models;
using System.Threading;
using VoidDays.ViewModels.Events;
using Prism.Events;
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
        public AdminService(IUnitOfWork unitOfWork, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _unitOfWork = unitOfWork;
            _dayRepository = _unitOfWork.DayRepository;
            _goalItemsCreatedRepository = _unitOfWork.GoalItemsCreatedRepository;
            _goalItemRepository = _unitOfWork.GoalItemRepository;
            _settingsRepository = _unitOfWork.SettingsRepository;
            _goalRepository = _unitOfWork.GoalRepository;
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
            var isCurrentDay  = CheckForCurrentDay(GetCurrentStoredDay(), out day2);
            day = day2;
            return isCurrentDay;
        }
        public Day CreateNextDay(Day currentDay)
        {
            currentDay.IsActive = false;

            var nextDay = new Day();
            var settings = GetSettings();
            nextDay.DayNumber = currentDay.DayNumber + 1;
            var startDateTime = new DateTime();
            startDateTime = currentDay.Start.Date;
            nextDay.Start = startDateTime.AddDays(1) + settings.StartTime;
            var addDay = nextDay.Start.AddDays(1);
            var subtractSecond = addDay.AddSeconds(-1);
            nextDay.End = subtractSecond;
            nextDay.IsActive = true;
            _dayRepository.Insert(nextDay);
            _unitOfWork.Save();

            //SyncToCurrentDay(currentDay);
            return nextDay;
        }

        public Day CreateFirstDay()
        {
            var firstDay = new Day();
            var settings = GetSettings();
            firstDay.DayNumber = 0;

            firstDay.Start = DateTime.UtcNow.Date + settings.StartTime;
            firstDay.End = firstDay.Start.Add(new TimeSpan(23, 59, 59));
            firstDay.IsActive = true;

            _dayRepository.Insert(firstDay);
            _unitOfWork.Save();

            //SyncToCurrentDay(currentDay);
            return firstDay;
        }

        public List<GoalItem> GetGoalItemsByDayNumber(int day)
        {
            var goalItems = _goalItemRepository.Get(x => x.DayNumber == day);
            return goalItems.ToList();
        }
        public void SaveGoalItem(GoalItem goalItem)
        {
            _goalItemRepository.Update(goalItem);
            goalItem.Goal.Message = goalItem.Message;
            goalItem.Goal.Title = goalItem.Title;
            _goalRepository.Update(goalItem.Goal);

            _unitOfWork.Save();
        }

        public Day SyncToCurrentDay(Day currentStoredDay)
        {
            //complete this day, foreach goalitem on this day not completed, void
            var goalItems = GetGoalItemsByDayNumber(currentStoredDay.DayNumber).ToList();
            foreach (var item in goalItems)
            {
                if (item.IsComplete == false)
                {
                    item.IsVoid = true;
                    currentStoredDay.IsVoid = true;
                }
                SaveGoalItem(item);
            }
            currentStoredDay.IsActive = false;

            var nextDay = CreateNextDay(currentStoredDay);

            CreateAllGoalItems(nextDay.DayNumber);
            Day current;
            if (!CheckForCurrentDay(nextDay, out current))
            {
                SyncToCurrentDay(current);
            }
            return nextDay;
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
            _unitOfWork.Save();
        }
        public List<Day> GetDaysByDayNumber(int start, int end)
        {
            return _dayRepository.Get(x => x.DayNumber >= start && x.DayNumber <= end).ToList();
        }
        public Day GetCurrentStoredDay()
        {
            return _dayRepository.Get().LastOrDefault();
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
            return settings;
        }

        public Day GetNextDay(Day day)
        {
            return _dayRepository.Get(x => x.DayNumber == day.DayNumber + 1).LastOrDefault();
        }

        private Timer timer;
        public Timer SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.UtcNow;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                SetUpTimer(alertTime.Add(new TimeSpan(24, 0, 0)));
                return timer;//time already passed
            }
            this.timer = new Timer(x =>
            {
                this.NextDayHandler();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
            return timer;
        }

        private void NextDayHandler()
        {
            timer = SetUpTimer(new TimeSpan(24, 0, 0));
            var day = SyncToCurrentDay(GetCurrentStoredDay());

            _eventAggregator.GetEvent<NextDayEvent>().Publish(day);
            //need to set next day here
        }
    }
}