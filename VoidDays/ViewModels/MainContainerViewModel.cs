using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.ViewModels.Interfaces;
using VoidDays.Services.Interfaces;
using System.Windows.Input;
using System.Threading;
using VoidDays.Models;
using Prism.Events;
using VoidDays.ViewModels.Events;
namespace VoidDays.ViewModels
{
    public class MainContainerViewModel : ViewModelBase, IMainContainerViewModel
    {
        IAdminService _adminService;
        Timer _timer;
        Settings _settings;
        IEventAggregator _eventAggregator;
        public MainContainerViewModel(IEventAggregator eventAggregator,ISmallHistoryDayViewModelContainer smallHistoryDayViewModelContainer, IMainViewContainerViewModel mainViewContainerViewModel, IDayHistoryViewModel dayHistoryViewModel, IAdminService adminService)
        {
            _eventAggregator = eventAggregator;
            _adminService = adminService;
            SetupTimer();

            SmallHistoryDayViewModelContainer = smallHistoryDayViewModelContainer;
            MainViewContainerViewModel = mainViewContainerViewModel;
            DayHistoryViewModel = dayHistoryViewModel;
            CurrentView = MainViewContainerViewModel;
            HistoryCommand = new ActionCommand(ShowHistory);
            CurrentDayCommand = new ActionCommand(ShowCurrentDay);
        }
        public ICommand HistoryCommand { get; set; }
        public ICommand CurrentDayCommand { get; set; }
        private void SetupTimer()
        {
            _settings = _adminService.GetSettings();
            _timer = _adminService.SetUpTimer(_settings.EndTime);
        }
        private void ShowHistory()
        {
            DayHistoryViewModel.Initialize();
            CurrentView = DayHistoryViewModel;
        }
        private void ShowCurrentDay()
        {
            _eventAggregator.GetEvent<SetListToTodayEvent>().Publish();
            CurrentView = MainViewContainerViewModel;
        }

        public object CurrentView { get; private set; }
        public IMainViewContainerViewModel MainViewContainerViewModel { get; private set; }
        public object SmallHistoryDayViewModelContainer { get; private set; }
        public IDayHistoryViewModel DayHistoryViewModel { get; set; }
    }
}
