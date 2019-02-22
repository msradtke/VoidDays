using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoidDays.Models;
using VoidDays.Services;

namespace VoidDays.ViewModels
{
    public class LoginSettingsViewModel : ViewModelBase, ILoginSettingViewModel
    {
        private readonly IUserService _userService;

        public LoginSettingsViewModel(IEventAggregator eventAggregator, IUserService userService)
        {
            _userService = userService;
            AppSettings = _userService.GetAppSettings();
            ServerAddress = AppSettings.ServerAddress;
            SetDefaultServerAddressCommand = new ActionCommand(SaveDefaultServerAddress);
        }

        private void SaveDefaultServerAddress()
        {
            _userService.SaveDefaultServerAddress(ServerAddress);
        }

        public ICommand SetDefaultServerAddressCommand { get; private set; }
        public AppSettings AppSettings{ get; set; }
        public string ServerAddress { get; set; }

    }

    public interface ILoginSettingViewModel : IViewModelBase
    {
        string ServerAddress { get; set; }
    }

    public interface ILoginSettingsViewModelFactory
    {
        LoginSettingsViewModel CreateLoginSettingsViewModel();
    }
}
