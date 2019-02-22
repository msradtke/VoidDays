﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.DTO;
using VoidDays.Models;
using VoidDays.Services;
using VoidDays.Services.Interfaces;
using VoidDays.ViewModels.Events;

namespace VoidDays.ViewModels
{
    public class StartupContainerViewModel : ViewModelBase, IStartupContainerViewModel
    {
        ILoginViewModelFactory _loginViewModelFactory;
        private readonly ICreateUserViewModelFactory _createUserViewModelFactory;
        private readonly ILoginSettingsViewModelFactory _loginSettingsViewModelFactory;
        private readonly IUserServiceFactory _userServiceFactory;
        private readonly IDialogService _dialogService;
        private readonly IDatabaseService _databaseService;
        private IUserService _userService;
        public StartupContainerViewModel(IEventAggregator eventAggregator,
            ILoginViewModelFactory loginViewModelFactory,
            ICreateUserViewModelFactory createUserViewModelFactory,
            ILoginSettingsViewModelFactory loginSettingsViewModelFactory,
            IUserServiceFactory userServiceFactory,
            IDialogService dialogService,
            IDatabaseService databaseService
            )
        {
            _eventAggregator = eventAggregator;
            _loginViewModelFactory = loginViewModelFactory;
            _createUserViewModelFactory = createUserViewModelFactory;
            _loginSettingsViewModelFactory = loginSettingsViewModelFactory;
            _userServiceFactory = userServiceFactory;
            _dialogService = dialogService;
            _databaseService = databaseService;
            TabItems = new ObservableCollection<IViewModelBase>();

            LoginViewModel = _loginViewModelFactory.CreateLoginViewModel();
            LoginViewModel.TabHeader = "Login";
            CreateUserViewModel = _createUserViewModelFactory.CreateCreateUserViewModel();
            CreateUserViewModel.TabHeader = "Create User";
            LoginSettingsViewModel = _loginSettingsViewModelFactory.CreateLoginSettingsViewModel();
            LoginSettingsViewModel.TabHeader = "Login Settings";
            _userService = _userServiceFactory.CreateUserService();

            

            _eventAggregator.GetEvent<TryLoginEvent>().Subscribe(TryLogin);
            _eventAggregator.GetEvent<TryCreateUserEvent>().Subscribe(CreateUser);
            AddTabItem(LoginViewModel);
            AddTabItem(CreateUserViewModel);
            AddTabItem(LoginSettingsViewModel);
        }

        private void TryLogin(LoginPayload payload)
        {
            var server = LoginSettingsViewModel.ServerAddress;
            string message = "";
            bool success = false;
            try
            {
                success = _userService.Login(payload.Username, payload.Password, server,out message);
            }
            catch (Exception e)
            {
                _dialogService.OpenErrorDialog(e.Message, "Login Failed");
                return;
            }
            if (success)
            {
                _eventAggregator.GetEvent<LoginSuccessEvent>().Publish();
                _userService.SetLastUser(payload.Username);   
            }
            else
            {
                _dialogService.OpenErrorDialog(message, "Error.");
            }
        }
        private void CreateUser(CreateUserPayload payload)
        {
            bool success = false;
            try
            {
                success = _userService.CreateUser(payload.Username, payload.Password);
            }
            catch
            {
                _dialogService.OpenErrorDialog("Create user failed.", "Error");
                return;
            }
            if (!success)
            {
                _dialogService.OpenErrorDialog("Create user failed.", "Error");
                return;
            }

            _eventAggregator.GetEvent<CreateUserSuccessEvent>().Publish(payload);

            LoginViewModel.Username = payload.Username;
            LoginViewModel.Password = "";
            SelectedTabItem = LoginViewModel;
        }
        
        public IViewModelBase CurrentView { get; set; }
        public ILoginViewModel LoginViewModel { get; set; }
        public IViewModelBase CreateUserViewModel { get; set; }
        public ILoginSettingViewModel LoginSettingsViewModel { get; set; }
        public AppSettings AppSettings { get; set; }

    }
    public interface IStartupContainerViewModel { }
    public interface IStartupContainerViewModelFactory
    {
        IStartupContainerViewModel CreateStartupContainerViewModel();
    }
}