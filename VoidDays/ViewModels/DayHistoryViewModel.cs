﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using VoidDays.Services.Interfaces;
using VoidDays.Models;
using VoidDays.ViewModels.Interfaces;
using System.Collections.ObjectModel;
namespace VoidDays.ViewModels
{
    [ImplementPropertyChanged]
    public class DayHistoryViewModel : IDayHistoryViewModel
    {
        IAdminService _adminService;
        IViewModelFactory _viewModelFactory;
        public DayHistoryViewModel(IAdminService adminService, IViewModelFactory viewModelFactory)
        {
            _adminService = adminService;
            _viewModelFactory = viewModelFactory;
            AllDays = _adminService.GetAllDays();
        }
        public List<Day> AllDays { get; set; }
        public List<List<Day>> Weeks { get; set; }
        public ObservableCollection<WeekViewModelAggregate> WeekViewModelAggregates { get; set; }

        public void Initialize()
        {
            Weeks = new List<List<Day>>();
            WeekViewModelAggregates = new ObservableCollection<WeekViewModelAggregate>();
            GetWeeks();
            CreateWeekViewModels();
        }
        private void GetWeeks()
        {
            var newWeek = new List<Day>();
            var sortedByDaysNumber = AllDays.OrderBy(x => x.DayNumber).ToList();
            bool weekAdded = false;
            foreach (var day in sortedByDaysNumber)
            {
                weekAdded = false;
                newWeek.Add(day);
                if (day.Start.DayOfWeek == DayOfWeek.Saturday)
                {
                    Weeks.Add(newWeek);
                    newWeek = new List<Day>();
                    weekAdded = true;
                }
            }
            if(!weekAdded)
                Weeks.Add(newWeek);

            
        }

        private void CreateWeekViewModels()
        {
            Weeks.Reverse();
            foreach (var week in Weeks)
            {

                var vmAggr = new WeekViewModelAggregate();
                vmAggr.WeekViewModel = _viewModelFactory.CreateDayHistoryWeekViewModel(week);
                vmAggr.WeekName = GetWeekName(week.OrderBy(x=>x.Start).FirstOrDefault());
                WeekViewModelAggregates.Add(vmAggr);
            }
        }

        private string GetWeekName(Day day)
        {
            if (day != null)
            {
                var startOfWeek = StartOfWeek(day.Start, DayOfWeek.Sunday);
                StringBuilder sb = new StringBuilder();
                sb.Append("Week of ");
                sb.Append(startOfWeek.ToString("MM/dd/yyyy"));
                return sb.ToString();
            }
            return "";
        }

        private static DateTime StartOfWeek( DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
    }

    public class WeekViewModelAggregate
    {
        public object WeekViewModel { get; set; }
        public string WeekName { get; set; }

    }
}