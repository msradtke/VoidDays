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
        IStartupServiceFactory _startupServiceFactory;
        private readonly IStartupContainerViewModelFactory _startupContainerViewModelFactory;

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

            CurrentView = _startupContainerViewModelFactory.CreateStartupContainerViewModel();
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
