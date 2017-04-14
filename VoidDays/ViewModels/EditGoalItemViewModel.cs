using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Services.Interfaces;
using VoidDays.Models;
using System.Windows.Input;
using System.Windows;
using VoidDays.ViewModels.Events;
using Prism.Events;
namespace VoidDays.ViewModels
{
    public class EditGoalItemViewModel : ViewModelBase
    {
        IDialogService _dialogService;
        IGoalService _goalService;
        public EditGoalItemViewModel(IEventAggregator eventAggregator, GoalItem goalItem, IDialogService dialogService, IGoalService goalService)
        {
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;
            _goalService = goalService;
            GoalItem = goalItem;
            SetGoalItemProperties();
            CancelCommand = new ActionCommand(Cancel);
            SaveCommand = new ActionCommand(Save);
            DeleteGoalCommand = new ActionCommand(DeleteGoal);
        }
        #region Commands
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteGoalCommand { get; set; }
        #endregion

        public GoalItem GoalItem { get; set; }
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
                GoalItem.Title = Title;
                GoalItem.Message = Message;
                _goalService.SaveGoalItem(GoalItem);
                _dialogService.CloseDialog(this);
            }

        }
        private void SetGoalItemProperties()
        {
            Title = GoalItem.Title;
            Message = GoalItem.Message;
        }
        private void DeleteGoal()
        {
            var result = _dialogService.OpenInquiryDialog("Are you sure you want to remove this goal?", "Delete Goal");
            if(result == MessageBoxResult.Yes)
            {
                _goalService.DeleteGoal(GoalItem);
                _eventAggregator.GetEvent<DeleteGoalItemEvent>().Publish(GoalItem);
            }
            _dialogService.CloseDialog(this);
        }
    }
}
