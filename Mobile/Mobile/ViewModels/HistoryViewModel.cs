using Mobile.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VoidDays.Contracts.Data;
using VoidDays.Core;
using System.Linq;
namespace Mobile.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        IVoidProvider _voidProvider;
        int _numWeeksShown = 2;
        int _currentWeekSection = 1;
        public HistoryViewModel()
        {
            _voidProvider = new VoidProvider();
            Days = _voidProvider.GetDays();
            SmallHistoryDayViewModel = new SmallHistoryDayViewModel(Days[0]);
            GetWeeks();
        }
        void GetWeeks()
        {
            WeekVms = new ObservableCollection<WeekViewModelAggregate>();

            var sectionDays = GetDaysForSection(_currentWeekSection);
            for(int i =1;i<=_numWeeksShown;i++)
            {
                var oneWeek = sectionDays.Take(7).ToList();
                var vm = new DayHistoryWeekViewModel(oneWeek);
                var vmAggregate = new WeekViewModelAggregate();
                vmAggregate.WeekViewModel = vm;
                vmAggregate.WeekName = GetWeekName(oneWeek.FirstOrDefault());
                WeekVms.Add(vmAggregate);
                sectionDays.RemoveRange(0, 7);
            }
        }
        List<DayDTO> GetDaysForSection(int section)
        {
            var sectionDays = new List<DayDTO>();
            var currentDayNumber = Days.Max(x => x.DayNumber);
            var startOfSectionDayNumber = currentDayNumber - (_numWeeksShown * section *7);
            for(int i =startOfSectionDayNumber;i<_numWeeksShown*7+ startOfSectionDayNumber; i++)
            {
                var day = Days.FirstOrDefault(x => x.DayNumber == i);
                if(day!=null)
                    sectionDays.Add(day);
            }

            return sectionDays;
        }
        private string GetWeekName(DayDTO day)
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
        private DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
        public List<DayDTO> Days { get; set; }
        public ObservableCollection<WeekViewModelAggregate> WeekVms { get; set; }
        public SmallHistoryDayViewModel SmallHistoryDayViewModel { get; set; }
    }
    public class WeekViewModelAggregate : BaseViewModel
    {
        public object WeekViewModel { get; set; }
        public string WeekName { get; set; }

    }
}
