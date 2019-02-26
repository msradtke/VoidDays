using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using VoidDays.Models;
using VoidDays.Services;
using VoidDays.ViewModels.Events;

namespace VoidDays.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        IMainContainerViewModelFactory _mainContainerViewModelFactory;
        IStartupServiceFactory _startupServiceFactory;
        private readonly IStartupContainerViewModelFactory _startupContainerViewModelFactory;
        private MainContainerViewModel _mainContainerViewModel;
        private Timer _timer;
        private readonly IncognitoViewModel _incognitoViewModel = new IncognitoViewModel();
        private bool _loggedIn = false;
        private bool _moveSwitch = false;
        public MainWindowViewModel(IEventAggregator eventAggregator,
            IMainContainerViewModelFactory mainContainerViewModelFactory,
            IStartupServiceFactory startupServiceFactory,
            IStartupContainerViewModelFactory startupContainerViewModelFactory)
        {
            _eventAggregator = eventAggregator;
            _mainContainerViewModelFactory = mainContainerViewModelFactory;
            _startupServiceFactory = startupServiceFactory;
            _startupContainerViewModelFactory = startupContainerViewModelFactory;
            LoadingViewModel = new LoadingViewModel();
            _eventAggregator.GetEvent<LoginSuccessEvent>().Subscribe(LoginSuccess);
            _eventAggregator.GetEvent<LogoutEvent>().Subscribe(Logout);

            CurrentView = _startupContainerViewModelFactory.CreateStartupContainerViewModel();
        }
        public void MouseMoved()
        {
            if (!_loggedIn)
                return;
            _timer.Stop();
            _timer.Start();
        }
        private void NewTimer()
        {
            _timer = new Timer();
            //this.timer = new Timer(timeToGo.Milliseconds);
            _timer = new Timer(30000);
            _timer.AutoReset = true;
            _timer.Elapsed += TimerElapsedHandler;
            _timer.Enabled = true;

        }
        public bool MoveSwitch { get; set; }
        private void TimerElapsedHandler(object o, ElapsedEventArgs e)
        {
            MoveSwitch = true;
            CurrentView = _incognitoViewModel;
        }
        public void SecretCodeEntered()
        {
            if (!_loggedIn)
                return;
            if (CurrentView != _mainContainerViewModel)
            {
                CurrentView = _mainContainerViewModel;

            }
        }
        private void Logout(bool obj)
        {
            _loggedIn = false;
            CurrentView = _startupContainerViewModelFactory.CreateStartupContainerViewModel();
        }

        public bool IsLoading { get; set; }
        public object CurrentView { get; set; }
        public object LoadingViewModel { get; set; }

        void LoginSuccess()
        {
            NewTimer();
            _loggedIn = true;
            _mainContainerViewModel = _mainContainerViewModelFactory.CreateMainContainerViewModel();

            CurrentView = _mainContainerViewModel;
            var startup = _startupServiceFactory.CreateStartupService();
            startup.Initialize();

            _mainContainerViewModel.Initialize();
        }
    }
}
