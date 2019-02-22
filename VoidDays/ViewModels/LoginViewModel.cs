using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoidDays.Models;
using VoidDays.Services;
using VoidDays.Services.Interfaces;
using VoidDays.ViewModels.Events;
using VoidDays.ServiceReference2;
using VoidDays.DTO;

namespace VoidDays.ViewModels
{
    public class LoginViewModel : ViewModelBase, ILoginViewModel
    {

        IUserService _userService;
        VoidDaysLoginServiceClient _loginClient;
        AppSettings _appSettings;
        public LoginViewModel(IEventAggregator eventAggregator, IUserService userService)
        {
            _eventAggregator = eventAggregator;
            _userService = userService;
            _appSettings = _userService.GetAppSettings();
            Username = _appSettings.LastUser;
            Password = "";
            LoginCommand = new ActionCommand(Login);
        }
        public ICommand LoginCommand { get; private set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string LoginMessage { get; set; }

        void Login()
        {
            _eventAggregator.GetEvent<TryLoginEvent>().Publish(new LoginPayload { Username = Username, Password = Password });
    
        }

        void CreateUser()
        {

        }
    }
    public interface ILoginViewModel : IViewModelBase
    {
        string Username { get; set; }
        string Password { get; set; }
    }
    public interface ILoginViewModelFactory
    {
        LoginViewModel CreateLoginViewModel();
    }
}
