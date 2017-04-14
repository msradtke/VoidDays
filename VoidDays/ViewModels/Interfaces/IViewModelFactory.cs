using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models;
namespace VoidDays.ViewModels.Interfaces
{
    public interface IViewModelFactory
    {
        GoalItemViewModel CreateGoalItemViewModel(GoalItem goalItem);
        EditGoalItemViewModel CreateEditGoalItemViewModel(GoalItem goalItem);
        CompleteGoalItemViewModel CreateCompleteGoalItemViewModel(GoalItem goalItem);
        AddNewGoalViewModel CreateAddNewGoalViewModel();
        SmallHistoryDayViewModel CreateSmallHistoryDayViewModel(Day day);
        DayHistoryWeekViewModel CreateDayHistoryWeekViewModel(List<Day> days);
    }
}
