﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Services.Interfaces;
using VoidDays.Models;
namespace VoidDays.Services
{
    public class StartupService : IStartupService
    {
        IAdminService _adminService;
        IGoalService _goalService;
        public StartupService(IAdminService adminService, IGoalService goalService)
        {
            _adminService = adminService;
            _goalService = goalService;
        }

        public void Initialize()
        {
            //do something to initalize first-time use

            //check days
            Day currentStoredDay;
            if (!_adminService.CheckForCurrentDay(out currentStoredDay))
            {
                _adminService.SyncToCurrentDay(currentStoredDay);
            }

            _goalService.SyncGoalItems(currentStoredDay.DayNumber);

        }
        /*private void SyncToCurrentDay(Day currentStoredDay)
        {
            //complete this day, foreach goalitem on this day not completed, void
            var goalItems = _goalService.GetGoalItemsByDayNumber(currentStoredDay.DayNumber).ToList();
            foreach (var item in goalItems)
            {
                if (item.IsComplete == false)
                {
                    item.IsVoid = true;
                    currentStoredDay.IsVoid = true;
                }
                _goalService.SaveGoalItem(item);
            }
            currentStoredDay.IsActive = false;
            var nextDay = _adminService.CreateNextDay();
            _goalService.CreateAllGoalItems(nextDay.DayNumber);
            Day current;
            if (!_adminService.CheckForCurrentDay(out current))
            { 
                SyncToCurrentDay(current);
            }
        }*/

        private void CreateDays(Day currentStoredDay)
        {
            //Finish current day if not finished
            //create void days for any completely missing days

        }


    }
}