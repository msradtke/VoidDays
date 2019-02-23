using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using VoidDays.Services.Interfaces;
using VoidDays.Models;
using VoidDays.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;
using VoidDays.ViewModels.Events;
using Prism.Events;

namespace VoidDays.ViewModels
{
    public class DayHistoryWeekViewModel : ViewModelBase
    {

        IViewModelFactory _viewModelFactory;
        public DayHistoryWeekViewModel(IEventAggregator eventAggregator, List<Day> days, IViewModelFactory viewModelFactory)
        {
            _eventAggregator = eventAggregator;
            _viewModelFactory = viewModelFactory;

            Days = days;

            DayViewModelAggregates = new ObservableCollection<DayViewModelAggregate>();
            DoubleClickDayCommand = new ActionCommand(DoubleClickDay, () => true);
            CreateDayViewModels();
        }

        private void DoubleClickDay(object dayVm)
        {
            var vm = (DayViewModelAggregate)dayVm;
            var day = vm.DayViewModel.Day;
            if (day != null)
                _eventAggregator.GetEvent<SetDayEvent>().Publish(day);
        }

        public ICommand DoubleClickDayCommand { get; set; }
        private void CreateDayViewModels()
        {
            var day = new Day();
            int nullDateCount = 1;
            Day lastDayBuffer = new Day();
            for (int i = 0; i < 7; ++i)
            {
                var dayOfWeek = (DayOfWeek)i;
                day = Days.FirstOrDefault(x => x.Start.DayOfWeek == dayOfWeek);



                var vm = _viewModelFactory.CreateSmallHistoryDayViewModel(day);

                if (day != null)
                {
                    lastDayBuffer = day;
                    var dayvm = new DayViewModelAggregate { DayName = day.Start.ToString("ddd"), DayViewModel = vm, DisplayDate = day.Start.ToShortDateString() };
                    DayViewModelAggregates.Add(dayvm);
                }
                else
                {
                    var nullDate = lastDayBuffer.Start.AddDays(nullDateCount);
                    var dayvm = new DayViewModelAggregate { DayName = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[i], DayViewModel = vm, DisplayDate = nullDate.ToShortDateString() };
                    DayViewModelAggregates.Add(dayvm);
                    ++nullDateCount;
                }
            }
        }
        public List<Day> Days { get; set; }
        public ObservableCollection<DayViewModelAggregate> DayViewModelAggregates { get; set; }
    }
    public class DayViewModelAggregate
    {
        public string DayName { get; set; }
        public string DisplayDate { get; set; }
        public SmallHistoryDayViewModel DayViewModel { get; set; }
    }
}