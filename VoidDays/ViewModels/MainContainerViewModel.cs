﻿using System;
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

        public MainContainerViewModel(IEventAggregator eventAggregator, Lazy<ISmallHistoryDayViewModelContainer> smallHistoryDayViewModelContainer, Lazy<IMainViewContainerViewModel> mainViewContainerViewModel, Lazy<IDayHistoryViewModel> dayHistoryViewModel, IAdminService adminService)
        {
            LoadingViewModel = new LoadingViewModel();
            _eventAggregator = eventAggregator;
            _adminService = adminService;
            _lazySmallHistoryDayViewModelContainer = smallHistoryDayViewModelContainer;
            _lazyMainViewContainerViewModel = mainViewContainerViewModel;
            _lazyDayHistoryViewModel = dayHistoryViewModel;




            HistoryCommand = new ActionCommand(ShowHistory);
            CurrentDayCommand = new ActionCommand(ShowCurrentDay);
            IsLoading = false;
            Initialize();
        }
        public ICommand HistoryCommand { get; set; }
        public ICommand CurrentDayCommand { get; set; }
        public void Initialize()
        {
            if (!IsLoading)
            {
                IsLoading = true;
                Task.Factory.StartNew(() =>
                {
                    MainViewContainerViewModel = _lazyMainViewContainerViewModel.Value;

                    _adminService.Initialize();
                    SmallHistoryDayViewModelContainer = _lazySmallHistoryDayViewModelContainer.Value;
                    DayHistoryViewModel = _lazyDayHistoryViewModel.Value;
                    SetupTimer();
                    CurrentView = MainViewContainerViewModel;
                    IsLoading = false;
                });
                
                
            }


        }
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
        public bool IsLoading { get; set; }
        public LoadingViewModel LoadingViewModel {get;set;}
    }
}
