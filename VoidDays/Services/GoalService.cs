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
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        IAdminService _adminService;
        public GoalService(IUnitOfWorkFactory unitOfWorkFactory, IAdminService adminService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _adminService = adminService;
        }
        public Goal GetGoalById(int id)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return unitOfWork.GoalRepository.GetByID(id);
            }
            
        }
        public List<GoalItem> GetCurrentGoalItems()
        {
            var currentDay = _adminService.GetCurrentStoredDay();
            return GetGoalItemsByDayNumber(currentDay.DayNumber);
        }
        public List<GoalItem> GetGoalItemsByDayNumber(int dayNumber)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var day = unitOfWork.DayRepository.Get(x => x.DayNumber == dayNumber).FirstOrDefault();
                if (day == null)
                    return GetGoalItemsForVoidDay(dayNumber,unitOfWork.GoalItemRepository,unitOfWork.GoalRepository,unitOfWork.DayRepository);
                var goalItems = unitOfWork.GoalItemRepository.Get(x => x.DayNumber == dayNumber);
                return goalItems.ToList();
            }
        }
        public List<GoalItem> GetGoalItemsForVoidDay(int dayNumber, IRepositoryBase<GoalItem> goalItemRepo, IRepositoryBase<Goal> goalRepo, IRepositoryBase<Day> dayRepo)
        {
            var voidGoalItems = new List<GoalItem>();
            var previousDay = dayRepo.Get(x => x.DayNumber < dayNumber).Max();
            var preivousDayGoals = goalRepo.Get(x => x.IsActive && x.Created >= x.Created);
            foreach(var goal in preivousDayGoals)
            {
                var goalItem = CreateGoalItem(goal, dayNumber, goalItemRepo);
                goalItem.IsVoid = true;
                voidGoalItems.Add(goalItem);
            }
            return voidGoalItems;
        }
        public List<GoalItem> SyncGoalItems(int dayNumber)
        {
            List<GoalItem> newGoalItems = new List<GoalItem>();
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var goalRepo = unitOfWork.GoalRepository;
                var goalItemRepo = unitOfWork.GoalItemRepository;

                var goals = goalRepo.Get(x => x.IsActive).ToList();
                var goalItems = goalItemRepo.Get(x => x.DayNumber == dayNumber);
                foreach (var goal in goals)
                {
                    if (goalItems.FirstOrDefault(x => x.GoalId == goal.GoalId) == null)
                    {
                        var newItem = CreateGoalItem(goal, dayNumber,goalItemRepo);
                        newGoalItems.Add(newItem);
                    }

                }
                unitOfWork.Save();// save created goal items
            }
            return newGoalItems;
        }

        public GoalItem CreateGoalItem(Goal goal, int dayNumber, IRepositoryBase<GoalItem> goalItemRepo)
        {
            var goalitem = new GoalItem();
            goalitem.Goal = goal;
            goalitem.IsComplete = false;
            goalitem.Message = goal.Message;
            goalitem.Title = goal.Title;
            goalitem.DayNumber = dayNumber;
            goalitem.DateTime = DateTime.UtcNow;

            goalItemRepo.Insert(goalitem);
            return goalitem;
        }
        public void SaveNewGoal(Goal goal)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                unitOfWork.GoalRepository.Insert(goal);
                unitOfWork.Save();
            }
        }
        public void SaveGoalItem(GoalItem goalItem)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                unitOfWork.GoalItemRepository.Update(goalItem);
                unitOfWork.Save();
            }
        }

        public void DeleteGoal(GoalItem goalItem)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var goal = unitOfWork.GoalRepository.GetByID(goalItem.GoalId);
                goal.IsActive = false;
                unitOfWork.GoalRepository.Update(goal);
                unitOfWork.Save();
            }
        }
        public void DeleteGoalItem(GoalItem goalItem, Day day)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var delete = unitOfWork.GoalItemRepository.Get(x => x.GoalId == goalItem.GoalId && x.DayNumber == day.DayNumber).FirstOrDefault();
                unitOfWork.GoalItemRepository.Delete(delete.GoalItemId);
                unitOfWork.Save();
            }
            
        }
    }
}
