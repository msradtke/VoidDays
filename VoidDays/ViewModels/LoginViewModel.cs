﻿using Prism.Events;
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
using VoidDays.VoidDaysLoginService;
namespace VoidDays.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        IStartupService _startupService;
        IUserService _userService;
        VoidDaysLoginServiceClient _loginClient;
        public LoginViewModel(IEventAggregator eventAggregator, IUserService userService, IStartupService startupService)
        {
            _eventAggregator = eventAggregator;
            _userService = userService;
            _startupService = startupService;

            _loginClient = new VoidDaysLoginServiceClient();
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
            var schema = _loginClient.LoginUser(Username, Password);
            if(schema == null)
                LoginMessage = "Invalid login.";
            var user = new User();
            
            _startupService.Initialize();
            _eventAggregator.GetEvent<LoginEvent>().Publish();
        }
    }
    public interface ILoginViewModelFactory
    {
        LoginViewModel CreateLoginViewModel();
    }
}
