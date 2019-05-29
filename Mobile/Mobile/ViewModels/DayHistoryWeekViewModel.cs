using Mobile.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
            CreateDayViewModels();
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

        private void CreateDayViewModels()
        {
            var day = new DayDTO();
            DayViewModelAggregates = new ObservableCollection<DayViewModelAggregate>();
            DayDTO firstNotNullDay = Days.FirstOrDefault(x => x.DayNumber == Days.Where(y => y != null).Min(y => x.DayNumber));
            int nullDateCount = 7 - Days.Count;
            for (int i = 0; i < 7; ++i)
            {
                var dayOfWeek = (DayOfWeek)i;

                day = Days.FirstOrDefault(x => x.Start.DayOfWeek == dayOfWeek);


                var vm = new SmallHistoryDayViewModel(day);

                if (day != null)
                {

                    var dayvm = new DayViewModelAggregate { DayName = day.Start.ToString("ddd"), DayViewModel = vm, DisplayDate = day.Start.ToShortDateString() };
                    DayViewModelAggregates.Add(dayvm);
                }
                else
                {
                    var nullDate = firstNotNullDay.Start.AddDays(-nullDateCount);
                    var dayvm = new DayViewModelAggregate { DayName = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[i], DayViewModel = vm, DisplayDate = nullDate.ToShortDateString() };
                    DayViewModelAggregates.Add(dayvm);
                    --nullDateCount;
                }
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
