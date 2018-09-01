using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using VoidDays.Services.Interfaces;
using VoidDays.Models;
using System.Windows.Input;
using Prism.Events;
using VoidDays.ViewModels.Events;
namespace VoidDays.ViewModels
{
    public class AddNewGoalViewModel : ViewModelBase
    {
        IDialogService _dialogService;
        IGoalService _goalService;
        public AddNewGoalViewModel(IDialogService dialogService, IGoalService goalService,IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _goalService = goalService;
            Goal = new Models.Goal();
            CancelCommand = new ActionCommand(Cancel);
            SaveCommand = new ActionCommand(Save);
        }
        #region Commands
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        #endregion

        public Goal Goal { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public void Cancel()
        {
            _dialogService.CloseDialog(this);
        }
        public void Save()
        {
            if (String.IsNullOrWhiteSpace(Title))
                _dialogService.OpenErrorDialog("Title cannot be empty.", "Error");
            else
            {
                
                Goal.Title = Title;
                Goal.Message = Message;
                Goal.Created = DateTime.UtcNow;
                Goal.IsActive = true;
                Goal.IsComplete = false;
                Goal.IsDeleted = false;

                _goalService.SaveNewGoal(Goal);
                //_eventAggregator.GetEvent<CurrentDayStatusEvent>().Publish(false);
                _dialogService.CloseDialog(this);
            }

        }
    }
}
