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
namespace VoidDays.ViewModels
{
    [ImplementPropertyChanged]
    public class SmallHistoryDayViewModelContainer : ISmallHistoryDayViewModelContainer
    {
        IAdminService _adminService;
        IViewModelFactory _viewModelFactory;
        Day _currentStoredDay;
        const int _previousDayCount = 7;
        int _firstDay;
        int _endDay;
        public SmallHistoryDayViewModelContainer(IAdminService adminService, IViewModelFactory viewModelFactory)
        {
            _adminService = adminService;
            _viewModelFactory = viewModelFactory;
            _currentStoredDay = _adminService.GetCurrentStoredDay();
            SmallHistoryDayViewModels = new ObservableCollection<object>();
            Days = new List<Day>();
            GetPreviousDays();
            SetSmallHistoryDayViewModels();
        }

        public List<Day> Days { get; set; }
        public ObservableCollection<object> SmallHistoryDayViewModels { get; set; }
        private void GetPreviousDays()
        {
            _firstDay = _currentStoredDay.DayNumber - _previousDayCount;
            _endDay = _currentStoredDay.DayNumber - 1;
            Days = _adminService.GetDaysByDayNumber(_firstDay, _endDay);
        }

        private void SetSmallHistoryDayViewModels()
        {
            for(int i = _firstDay; i<= _endDay; ++i)
            {
                object vm;
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
