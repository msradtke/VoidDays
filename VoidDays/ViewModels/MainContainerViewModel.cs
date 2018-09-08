using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.ViewModels.Interfaces;
using VoidDays.Services.Interfaces;
using System.Windows.Input;
using System.Timers;
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
        Lazy<ISmallHistoryDayViewModelContainer> _lazySmallHistoryDayViewModelContainer;
        Lazy<IMainViewContainerViewModel> _lazyMainViewContainerViewModel;
        Lazy<IDayHistoryViewModel> _lazyDayHistoryViewModel;
        List<LoadingLock> _loadingLocks;
        public MainContainerViewModel(IEventAggregator eventAggregator, Lazy<ISmallHistoryDayViewModelContainer> smallHistoryDayViewModelContainer, Lazy<IMainViewContainerViewModel> mainViewContainerViewModel, Lazy<IDayHistoryViewModel> dayHistoryViewModel, IAdminService adminService)
        {
            LoadingViewModel = new LoadingViewModel();
            IsLoading = true;

            _eventAggregator = eventAggregator;
            _adminService = adminService;
            _lazySmallHistoryDayViewModelContainer = smallHistoryDayViewModelContainer;
            _lazyMainViewContainerViewModel = mainViewContainerViewModel;
            _lazyDayHistoryViewModel = dayHistoryViewModel;            
        }
        public ICommand HistoryCommand { get; set; }
        public ICommand CurrentDayCommand { get; set; }
        public void Initialize()
        {
            _eventAggregator.GetEvent<LoadingEvent>().Subscribe(LoadingEventListener);
            _loadingLocks = new List<LoadingLock>();

            HistoryCommand = new ActionCommand(ShowHistory);
            CurrentDayCommand = new ActionCommand(ShowCurrentDay);
            IsLoading = false;

            //if (!IsLoading)
            {
                var loadLock = new LoadingLock { Id = Guid.NewGuid(), IsLoading = true };
                AddLoadLock(loadLock);
                IsLoading = true;
                Task.Factory.StartNew(() =>
                {
                    MainViewContainerViewModel = _lazyMainViewContainerViewModel.Value;

                    _adminService.Initialize();
                    SmallHistoryDayViewModelContainer = _lazySmallHistoryDayViewModelContainer.Value;
                    DayHistoryViewModel = _lazyDayHistoryViewModel.Value;
                    SetupTimer();
                    CurrentView = MainViewContainerViewModel;
                    RemoveLoadLock(loadLock);
                });
            }
        }

        private void AddLoadLock(LoadingLock loadLock)
        {
            if (loadLock.IsLoading)
            {
                if (!_loadingLocks.Exists(x => x.Id == loadLock.Id))
                {
                    _loadingLocks.Add(loadLock);
                    IsLoading = true;
                }
            }
            else
            {
                if (!_loadingLocks.Exists(x => x.Id == loadLock.Id))
                    RemoveLoadLock(loadLock);
            }
        }
        private void LoadingEventListener(LoadingLock loadLock)
        {
            if (loadLock.IsLoading == true)
                AddLoadLock(loadLock);
            else
                RemoveLoadLock(loadLock);
        }
        private void RemoveLoadLock(LoadingLock loadlLock)
        {            
            _loadingLocks.Remove(loadlLock);
            if (_loadingLocks.Count == 0)
                IsLoading = false;
        }
        private void SetupTimer()
        {
            _settings = _adminService.GetSettings();
            _timer = _adminService.SetUpTimer(_settings.EndTime.TimeOfDay);
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
        public bool IsLoading { get; set; }
        public LoadingViewModel LoadingViewModel {get;set;}
        public User User { get; set; }
    }

    public interface IMainContainerViewModelFactory
    {
        MainContainerViewModel CreateMainContainerViewModel();
    }
}
