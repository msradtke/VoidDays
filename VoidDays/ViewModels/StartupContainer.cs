using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.ViewModels
{
    public class StartupContainer : ViewModelBase
    {
        ILoginViewModelFactory _loginViewModelFactory;
        private readonly ICreateUserViewModelFactory _createUserViewModelFactory;

        public StartupContainer(IEventAggregator eventAggregator,
            ILoginViewModelFactory loginViewModelFactory, 
            ICreateUserViewModelFactory createUserViewModelFactory
            )
        {
            _eventAggregator = eventAggregator;
            _loginViewModelFactory = loginViewModelFactory;
            _createUserViewModelFactory = createUserViewModelFactory;

        }
    }
}
