using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Services.Interfaces;
using VoidDays.Models;
using VoidDays.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using PropertyChanged;
using Prism.Events;
using VoidDays.ViewModels.Events;
namespace VoidDays.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SmallHistoryDayViewModelContainer : ViewModelBase, ISmallHistoryDayViewModelContainer
    {
        IAdminService _adminService;
        IViewModelFactory _viewModelFactory;
        Day _currentStoredDay;
        const int _previousDayCount = 7;
        int _firstDay;
        int _endDay;
        IEventAggregator _eventAggregator;
        public SmallHistoryDayViewModelContainer(IAdminService adminService, IViewModelFactory viewModelFactory, IEventAggregator eventAggregator)
        {
            _adminService = adminService;
            _viewModelFactory = viewModelFactory;
            _currentStoredDay = _adminService.GetCurrentStoredDay();

            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<NextDayEvent>().Subscribe(NextDayEventHandler);
            //_eventAggregator.GetEvent<CheckNextDayEvent>().Subscribe(NextDayEventHandler);
            Days = new List<Day>();
            GetPreviousDays();
            SetSmallHistoryDayViewModels();
        }

        public List<Day> Days { get; set; }
        public ObservableCollection<ViewModelBase> SmallHistoryDayViewModels { get; set; }

        private void GetPreviousDays()
        {
            _firstDay = _currentStoredDay.DayNumber - _previousDayCount;
            if (_firstDay < 0)
                _firstDay = 0;
            _endDay = _currentStoredDay.DayNumber - 1;
            if (_endDay < 0)
                _endDay = 0;

            Days = _adminService.GetVoidDayRange(_firstDay, _endDay);
        }
        private void NextDayEventHandler(Day NextDay)
        {

            App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
            {
                _currentStoredDay = NextDay;
                GetPreviousDays();
                SetSmallHistoryDayViewModels();
            });


        }
        private void SetSmallHistoryDayViewModels()
        {
            SmallHistoryDayViewModels = new ObservableCollection<ViewModelBase>();
            for (int i = _firstDay; i <= _endDay; ++i)
            {
                ViewModelBase vm;
                var day = Days.FirstOrDefault(x => x.DayNumber == i);
                if (day == null)
                {
                    vm = _viewModelFactory.CreateSmallHistoryDayViewModel(null);
                }
                else
                    vm = _viewModelFactory.CreateSmallHistoryDayViewModel(day);
                SmallHistoryDayViewModels.Add(vm);
            }
        }
    }
}
