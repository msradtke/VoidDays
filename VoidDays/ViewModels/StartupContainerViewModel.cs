using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.ViewModels
{
    public class StartupContainerViewModel : ViewModelBase, IStartupContainerViewModel
    {
        ILoginViewModelFactory _loginViewModelFactory;
        private readonly ICreateUserViewModelFactory _createUserViewModelFactory;

        public StartupContainerViewModel(IEventAggregator eventAggregator,
            ILoginViewModelFactory loginViewModelFactory, 
            ICreateUserViewModelFactory createUserViewModelFactory
            )
        {
            _eventAggregator = eventAggregator;
            _loginViewModelFactory = loginViewModelFactory;
            _createUserViewModelFactory = createUserViewModelFactory;
            CurrentView = _loginViewModelFactory.CreateLoginViewModel();
        }

        public ViewModelBase CurrentView { get; set; }
    }
    public interface IStartupContainerViewModel { }
    public interface IStartupContainerViewModelFactory
    {
        IStartupContainerViewModel CreateStartupContainerViewModel();
    }
}
