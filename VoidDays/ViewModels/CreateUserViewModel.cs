using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VoidDays.ViewModels.Events;

namespace VoidDays.ViewModels
{
    public class CreateUserViewModel : ViewModelBase, ICreateUserViewModel
    {
        public CreateUserViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            CreateUserCommand = new ActionCommand(CreateUser);
        }
        public ICommand CreateUserCommand { get; set; }
        public string SecurePassword { private get; set; }
        public string SecureVerifyPassword { private get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        void CreateUser()
        {
            if (SecurePassword != SecureVerifyPassword)
            {
                Message = "Passwords must match";
                return;
            }
            Message = "";
            _eventAggregator.GetEvent<TryCreateUserEvent>().Publish(new DTO.CreateUserPayload { Username = Username, Password = SecurePassword});
        }
    }

    public interface ICreateUserViewModel { }
    public interface ICreateUserViewModelFactory
    {
        CreateUserViewModel CreateCreateUserViewModel();
    }
}
