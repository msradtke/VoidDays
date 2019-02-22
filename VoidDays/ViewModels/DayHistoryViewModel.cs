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
using Prism.Events;
using VoidDays.ViewModels.Events;
namespace VoidDays.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class DayHistoryViewModel : IDayHistoryViewModel
    {
        IAdminService _adminService;
        IViewModelFactory _viewModelFactory;
        IEventAggregator _eventAggregator;
        public DayHistoryViewModel(IEventAggregator eventAggregator, IAdminService adminService, IViewModelFactory viewModelFactory)
        {
            LoadingViewModel = new LoadingViewModel();

            _adminService = adminService;
            _adminService.Initialize();
            _viewModelFactory = viewModelFactory;

            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<NextDayEvent>().Subscribe(NextDayEventHandler);

        }
        public List<Day> AllDays { get; set; }

        public List<List<Day>> Weeks { get; set; }
        public ObservableCollection<WeekViewModelAggregate> WeekViewModelAggregates { get; set; }
        public bool IsLoading { get; set; }
        public LoadingViewModel LoadingViewModel { get; set; }
        public void Initialize()
        {
            if (IsLoading == false)
            {
                IsLoading = true;
                //AllDays = _adminService.GetAllDays();
                Weeks = new List<List<Day>>();
                WeekViewModelAggregates = new ObservableCollection<WeekViewModelAggregate>();
                Task.Factory.StartNew
                    (() =>
                    {
                        AllDays = _adminService.GetAllVoidDays();
                        Weeks = new List<List<Day>>();
                        WeekViewModelAggregates = new ObservableCollection<WeekViewModelAggregate>();
                        GetWeeks();
                        CreateWeekViewModels();
                        //System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(5000));
                    }
                    )
                    .ContinueWith(x => IsLoading = false);
            }
            //GetWeeks();
            //CreateWeekViewModels();
        }
        private void NextDayEventHandler(Day nextDay)
        {
            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                Initialize();
            });

        }
        private void GetWeeks()
        {
            var newWeek = new List<Day>();
            var sortedByDaysNumber = AllDays.OrderBy(x => x.DayNumber).ToList();
            var highestDayNumber = AllDays.Max(x => x.DayNumber);

            bool weekAdded = false;
            Day previousDay = null;
            for (int i = 0; i <= highestDayNumber; ++i)
            {
                var day = sortedByDaysNumber.FirstOrDefault(x => x.DayNumber == i);
                if (day == null)
                {
                    if (previousDay == null)
                        throw new InvalidOperationException();
                    day = _adminService.GetVoidDayAfter(previousDay);
                }
                weekAdded = false;
                newWeek.Add(day);
                if (day.Start.DayOfWeek == DayOfWeek.Saturday)
                {
                    Weeks.Add(newWeek);
                    newWeek = new List<Day>();
                    weekAdded = true;
                }

                previousDay = day;
            }

            if (!weekAdded)
                Weeks.Add(newWeek);
        }

        private void CreateWeekViewModels()
        {
            Weeks.Reverse();
            foreach (var week in Weeks)
            {

                var vmAggr = new WeekViewModelAggregate();
                vmAggr.WeekViewModel = _viewModelFactory.CreateDayHistoryWeekViewModel(week);
                vmAggr.WeekName = GetWeekName(week.OrderBy(x => x.Start).FirstOrDefault());
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    WeekViewModelAggregates.Add(vmAggr);
                });

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

        private static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
    }
    [AddINotifyPropertyChangedInterface]
    public class WeekViewModelAggregate
    {
        public object WeekViewModel { get; set; }
        public string WeekName { get; set; }

    }
}
