using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models;
namespace VoidDays.Services.Interfaces
{
    public interface IGoalService
    {
        List<GoalItem> GetCurrentGoalItems();
        void SaveGoalItem(GoalItem goalItem);
        List<GoalItem> GetGoalItemsByDayNumber(int dayNumber);
        void CreateAllGoalItems(int dayNumber);
        void SaveNewGoal(Goal goal);
        GoalItem CreateGoalItem(Goal goal, int dayNumber);
        List<GoalItem> SyncGoalItems(int dayNumber);
        void DeleteGoalItem(GoalItem goalItem, Day day);
        void DeleteGoal(GoalItem goalItem);
    }
}
