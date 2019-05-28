using Mobile.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VoidDays.Contracts.Data;

namespace Mobile.ViewModels
{
    public class DayHistoryWeekViewModel : BaseViewModel
    {
        VoidProvider _voidProvider;
        public DayHistoryWeekViewModel(List<DayDTO> days)
        {
            _voidProvider = new VoidProvider();
            Days = days;
            CreateViewModels();
        }

        void CreateViewModels()
        {
            DayViewModelAggregates = new ObservableCollection<DayViewModelAggregate>();
            foreach (var day in Days)
            {
                var vm = new DayViewModelAggregate();
                vm.DayViewModel = new SmallHistoryDayViewModel(day);
                vm.DayName = day.Start.DayOfWeek.ToString();
                vm.DisplayDate = day.Start.ToShortDateString();
                DayViewModelAggregates.Add(vm);
            }
        }

        public List<DayDTO> Days { get; set; }
        public ObservableCollection<DayViewModelAggregate> DayViewModelAggregates { get; set; }
    }
    public class DayViewModelAggregate : BaseViewModel
    {
        public string DayName { get; set; }
        public string DisplayDate { get; set; }
        public SmallHistoryDayViewModel DayViewModel { get; set; }
    }
}
