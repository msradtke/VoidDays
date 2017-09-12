using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models;
using VoidDays.Models.Interfaces;
using VoidDays.Services.Interfaces;
namespace VoidDays.Services
{
    public class GoalService : IGoalService
    {
        IUnitOfWork _unitOfWork;
        IRepositoryBase<GoalItem> _goalItemRepository;
        IRepositoryBase<Goal> _goalRepository;
        IAdminService _adminService;
        public GoalService(IUnitOfWork unitOfWork, IAdminService adminService)
        {
            _unitOfWork = unitOfWork;
            _goalItemRepository = _unitOfWork.GoalItemRepository;
            _goalRepository = _unitOfWork.GoalRepository;
            _adminService = adminService;
        }
        public Goal GetGoalById(int id)
        {
            return _goalRepository.GetByID(id);
        }
        public List<GoalItem> GetCurrentGoalItems()
        {
            var currentDay = _adminService.GetCurrentStoredDay();
            return GetGoalItemsByDayNumber(currentDay.DayNumber);
        }
        public List<GoalItem> GetGoalItemsByDayNumber(int day)
        {
            var goalItems = _goalItemRepository.Get(x => x.DayNumber == day);
            return goalItems.ToList();
        }
        public void SaveGoalItem(GoalItem goalItem)
        {
            var gi = _goalItemRepository.GetByID(goalItem.GoalItemId);

            _goalItemRepository.Update(gi);
            gi.IsComplete = goalItem.IsComplete;
            gi.Goal.Message = goalItem.Goal.Message;
            gi.Goal.Title = goalItem.Goal.Title;
            gi.CompleteMessage = goalItem.CompleteMessage;
            _goalRepository.Update(gi.Goal);
            _unitOfWork.Save();
        }
        public void CreateAllGoalItems(int dayNumber)
        {
            var goals = _goalRepository.Get(x => x.IsActive).ToList();
            foreach (var goal in goals)
            {
                var goalitem = new GoalItem();
                goalitem.Goal = goal;
                goalitem.IsComplete = false;
                goalitem.Message = goal.Message;
                goalitem.Title = goal.Title;
                goalitem.DayNumber = dayNumber;
                goalitem.DateTime = DateTime.UtcNow;


                _goalItemRepository.Insert(goalitem);
            }
            _unitOfWork.Save();
        }
        public List<GoalItem> SyncGoalItems(int dayNumber)
        {
            var newGoalItems = new List<GoalItem>();
            var goals = _goalRepository.Get(x => x.IsActive).ToList();
            var goalItems = _goalItemRepository.Get(x => x.DayNumber == dayNumber);
            foreach (var goal in goals)
            {
                if(goalItems.FirstOrDefault(x => x.GoalId == goal.GoalId) == null)
                {
                    var newItem = CreateGoalItem(goal, dayNumber);
                    newGoalItems.Add(newItem);
                }
                
            }
            return newGoalItems;
        }
        public GoalItem CreateGoalItem(Goal goal, int dayNumber)
        {
            var goalitem = new GoalItem();
            goalitem.Goal = goal;
            goalitem.IsComplete = false;
            goalitem.Message = goal.Message;
            goalitem.Title = goal.Title;
            goalitem.DayNumber = dayNumber;
            goalitem.DateTime = DateTime.UtcNow;

            _goalItemRepository.Insert(goalitem);
            _unitOfWork.Save();
            return goalitem;
        }
        public void SaveNewGoal(Goal goal)
        {
            _goalRepository.Insert(goal);
            _unitOfWork.Save();
        }

        public void DeleteGoal(GoalItem goalItem)
        {
            var goal = _goalRepository.GetByID(goalItem.GoalId);
            goal.IsActive = false;
            _goalRepository.Update(goal);
            _unitOfWork.Save();
        }
        public void DeleteGoalItem(GoalItem goalItem, Day day)
        {
            var delete = _goalItemRepository.Get(x => x.GoalId == goalItem.GoalId && x.DayNumber == day.DayNumber).FirstOrDefault();
            _goalItemRepository.Delete(delete.GoalItemId);
            _unitOfWork.Save();
        }
    }
}
