using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoidDays.Models;
using VoidDays.Services;
using VoidDays.ViewModels.Events;

namespace VoidDays.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        IUserService _userService;
        public LoginViewModel(IEventAggregator eventAggregator, IUserService userService)
        {
            _eventAggregator = eventAggregator;
            _userService = userService;
            Username = "";
            Password = "";
            LoginCommand = new ActionCommand(Login);
        }
        public ICommand LoginCommand { get; private set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string LoginMessage { get; set; }

        void Login()
        {
            if (_userService.Login(Username, Password, out User user))
            {
                _eventAggregator.GetEvent<LoginEvent>().Publish(user);
            }
            else
                LoginMessage = "Invalid login.";
        }
    }
    public interface ILoginViewModelFactory
    {
        LoginViewModel CreateLoginViewModel();
    }
}
