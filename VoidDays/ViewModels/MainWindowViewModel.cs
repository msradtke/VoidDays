using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models;
using VoidDays.Services;
using VoidDays.ViewModels.Events;

namespace VoidDays.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        IMainContainerViewModelFactory _mainContainerViewModelFactory;
        ILoginViewModelFactory _loginViewModelFactory;
        IStartupServiceFactory _startupServiceFactory;

        public MainWindowViewModel(IEventAggregator eventAggregator,
            IMainContainerViewModelFactory mainContainerViewModelFactory,
            ILoginViewModelFactory loginViewModelFactory,
            IStartupServiceFactory startupServiceFactory)
        {
            _eventAggregator = eventAggregator;
            _mainContainerViewModelFactory = mainContainerViewModelFactory;
            _loginViewModelFactory = loginViewModelFactory;
            _startupServiceFactory = startupServiceFactory;
            LoadingViewModel = new LoadingViewModel();
            _eventAggregator.GetEvent<LoginEvent>().Subscribe(LoginSuccess);

            CurrentView = _loginViewModelFactory.CreateLoginViewModel();
        }

        public bool IsLoading { get; set; }
        public object CurrentView { get; set; }
        public object LoadingViewModel { get; set; }

        void LoginSuccess()
        {
            var vm = _mainContainerViewModelFactory.CreateMainContainerViewModel();
            
            CurrentView = vm;
            var startup = _startupServiceFactory.CreateStartupService();
            startup.Initialize();

            vm.Initialize();
        }
    }
}
