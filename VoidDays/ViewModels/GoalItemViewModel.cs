using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.ViewModels.Interfaces;
using VoidDays.Models;
namespace VoidDays.ViewModels
{
    public class GoalItemViewModel : ViewModelBase, IGoalItemViewModel
    {
        public GoalItemViewModel()
        {

        }
        public GoalItemViewModel(GoalItem goalItem)
        {
            GoalItem = goalItem;
        }
        public GoalItem GoalItem { get; set; }
    }
}
