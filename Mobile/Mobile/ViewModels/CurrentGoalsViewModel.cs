using Mobile.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VoidDays.Contracts.Data;
using VoidDays.Core;
namespace Mobile.ViewModels
{
    public class CurrentGoalsViewModel
    {
        VoidProvider _provider;
        public CurrentGoalsViewModel()
        {
            _provider = new VoidProvider();
            GoalItems = _provider.GetCurrentGoalItems().ToObservableCollection();

        }

        public DayDTO CurrentDay { get; set; }
        public ObservableCollection<GoalItemDTO> GoalItems { get; set; }

    }
}
