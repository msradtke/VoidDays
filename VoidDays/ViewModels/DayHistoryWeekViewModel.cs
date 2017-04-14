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
    public class DayHistoryWeekViewModel
    {
        
        IViewModelFactory _viewModelFactory;
        public DayHistoryWeekViewModel(List<Day> days, IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;

            Days = days;

            DayViewModelAggregates = new ObservableCollection<DayViewModelAggregate>();

            CreateDayViewModels();
        }

        private void CreateDayViewModels()
        {
            var day = new Day();
            
            for(int i = 0; i < 7; ++i)
            {
                var dayOfWeek = (DayOfWeek)i;
                day = Days.FirstOrDefault(x => x.Start.DayOfWeek == dayOfWeek);
                if (day == null)
                    day = null;

                var vm = _viewModelFactory.CreateSmallHistoryDayViewModel(day);
                var dayvm = new DayViewModelAggregate { DayName = dayOfWeek.ToString(), DayViewModel = vm };
                DayViewModelAggregates.Add(dayvm);
            }
        }
        public List<Day> Days { get; set; }
        public ObservableCollection<DayViewModelAggregate> DayViewModelAggregates { get; set; }
    }
    public class DayViewModelAggregate
    {
        public string DayName { get; set; }
        public object DayViewModel { get; set; }
    }
}