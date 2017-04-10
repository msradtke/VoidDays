using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models.Interfaces;
namespace VoidDays.Models
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {

        private IDbContext _context;
        private IRepositoryBaseFactory _repositoryBaseFactory;
        private IRepositoryBase<Goal> _goalRepository;
        public UnitOfWork(IRepositoryBaseFactory repositoryBaseFactory, IDbContext context)
        {
            _repositoryBaseFactory = repositoryBaseFactory;
            _context = context;

        }
        public IRepositoryBase<Goal> GoalRepository
        {
            get
            {

                if (this._goalRepository == null)
                {
                    this._goalRepository = _repositoryBaseFactory.CreateGoalRepository(_context);
                }
                return _goalRepository;
            }
        }

        public void Save()
        {
            _context.Save();
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
    public interface IRepositoryBaseFactory
    {
        RepositoryBase<Goal> CreateGoalRepository(IDbContext context);
        RepositoryBase<GoalHistory> CreateGoalHistoryRepository(IDbContext context);
    }
}