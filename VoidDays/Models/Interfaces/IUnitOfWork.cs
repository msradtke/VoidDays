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

        IRepositoryBase<Goal> GoalRepository { get;}
    }
}
