using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models;
using VoidDays.Models.Interfaces;

namespace VoidDays.Debug
{
    public static class DebugService
    {
        private static IUnitOfWork _unitOfWork;
        private static IRepositoryBase<Day> _dayRepository;
        private static IRepositoryBase<GoalItemsCreated> _goalItemsCreatedRepository;
        private static IRepositoryBase<Settings> _settingsRepository;
        private static IRepositoryBase<GoalItem> _goalItemRepository;
        private static IRepositoryBase<Goal> _goalRepository;

        private static IKernel _container;

        static DebugService()
        {
            
        }
        public static void InitSettings()
        {
            if (_settingsRepository == null)
                return;

            var settings = new Settings();
            settings.StartDay = DateTime.Today.ToUniversalTime();
            settings.StartTime = new DateTime(2000, 1, 1, 8, 0, 0);
            settings.EndTime = new DateTime(2000, 1, 1, 7, 59, 59);
            settings.IsUpdating = false;

            _settingsRepository.Insert(settings);
            _unitOfWork.Save();            
        }

        public static void Initialize(IKernel container)
        {
            if (container == null)
                return;
            _container = container;

            _unitOfWork = _container.Get<IUnitOfWork>();

            _settingsRepository = _unitOfWork.SettingsRepository;
        }
        
    }
}
