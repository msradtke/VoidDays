using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidDays.Models.Interfaces
{
    public interface IUnitOfWork
    {

        void Save();
        void Dispose();
        void Reload(object entity);

        IRepositoryBase<Goal> GoalRepository { get;}
        IRepositoryBase<GoalItem> GoalItemRepository { get; }
        IRepositoryBase<Day> DayRepository { get; }
        IRepositoryBase<GoalItemsCreated> GoalItemsCreatedRepository { get; }
        IRepositoryBase<Settings> SettingsRepository { get; }
    }
}
