using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.ViewModels.Interfaces;
using VoidDays.Models;
using VoidDays.ViewModels.Events;
using VoidDays.Services.Interfaces;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Prism.Events;
namespace VoidDays.ViewModels
{
    public class CompleteGoalItemViewModel
    {
        IDialogService _dialogService;
        IGoalService _goalService;
        IEventAggregator _eventAggregator;
        public CompleteGoalItemViewModel(IEventAggregator eventAggregator, GoalItem goalItem, IDialogService dialogService, IGoalService goalService)
        {
            _dialogService = dialogService;
            _goalService = goalService;
            GoalItem = goalItem;
            _eventAggregator = eventAggregator;


            CompleteCommand = new ActionCommand(Complete);
            CancelCommand = new ActionCommand(Cancel);
            UndoCompleteCommand = new ActionCommand(UndoComplete);
            //SaveCommand = new ActionCommand(Save);
            SatisfactionLevels = new ObservableCollection<int>();
            SetSatisfactionLevels();

            Initialize();


        }
        #region Commands
        public ICommand CancelCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CompleteCommand { get; set; }
        public ICommand UndoCompleteCommand { get; set; }
        #endregion

        public GoalItem GoalItem { get; set; }
        public int SatisfactionLevel { get; set; }
        public ObservableCollection<int> SatisfactionLevels { get; set; }
        public string CompleteMessage { get; set; }
        public bool IsComplete { get; set; }

        private void Initialize()
        {
            if (GoalItem.IsComplete)
            {
                IsComplete = true;
                SatisfactionLevel = GoalItem.SatisfyScale;
                CompleteMessage = GoalItem.CompleteMessage;
            }
            else
            {
                IsComplete = false;
                SatisfactionLevel = 3;

            }




        }
        public void Cancel()
        {
            _dialogService.CloseDialog(this);
        }
        public void Complete()
        {
            if (!IsComplete)
            {
                GoalItem.IsComplete = true;
                GoalItem.CompleteMessage = CompleteMessage;
                GoalItem.SatisfyScale = SatisfactionLevel;
                _goalService.SaveGoalItem(GoalItem);
                _dialogService.CloseDialog(this);
                _eventAggregator.GetEvent<GoalItemStatusChange>().Publish(GoalItem);
            }
        }

        public void UndoComplete()
        {
            if (IsComplete)
            {
                GoalItem.IsComplete = false;
                GoalItem.CompleteMessage = "";
                GoalItem.SatisfyScale = 3;
                _goalService.SaveGoalItem(GoalItem);
                _dialogService.CloseDialog(this);
                _eventAggregator.GetEvent<GoalItemStatusChange>().Publish(GoalItem);
            }
        }

        private void SetSatisfactionLevels()
        {
            for(int i=0;i<6;++i)
                SatisfactionLevels.Add(i);
            
        }
    }
}
