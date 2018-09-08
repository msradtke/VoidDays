using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models;
using VoidDays.ViewModels.Events;

namespace VoidDays.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        IMainContainerViewModelFactory _mainContainerViewModelFactory;
        ILoginViewModelFactory _loginViewModelFactory;

        public MainWindowViewModel(IEventAggregator eventAggregator,
            IMainContainerViewModelFactory mainContainerViewModelFactory,
            ILoginViewModelFactory loginViewModelFactory)
        {
            _eventAggregator = eventAggregator;
            _mainContainerViewModelFactory = mainContainerViewModelFactory;
            _loginViewModelFactory = loginViewModelFactory;
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
            vm.Initialize();
        }
    }
}
