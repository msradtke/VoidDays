using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models.Interfaces;
using VoidDays.Services;
using VoidDays.Services.Interfaces;

namespace VoidDays.Models
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        IDbContextFactory _contextFactory;
        private IDbContext _context;
        private IRepositoryBaseFactory _repositoryBaseFactory;
        IDatabaseService _databaseService;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private IRepositoryBase<Goal> _goalRepository;
        private IRepositoryBase<GoalItem> _goalItemRepository;
        private IRepositoryBase<Day> _dayRepository;
        private IRepositoryBase<GoalItemsCreated> _goalItemsCreatedRepository;
        private IRepositoryBase<Settings> _settingsRepository;

        public UnitOfWork(IRepositoryBaseFactory repositoryBaseFactory,
            IDbContextFactory contextFactory,
            IDatabaseService databaseService,
            IUnitOfWorkFactory unitOfWorkFactory)
        {
            _repositoryBaseFactory = repositoryBaseFactory;
            _contextFactory = contextFactory;
            _databaseService = databaseService;
            _unitOfWorkFactory = unitOfWorkFactory;
            var connectionString = _databaseService.ConnectionString;
            _context = _contextFactory.CreateDbContext(connectionString);

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
        public IRepositoryBase<GoalItem> GoalItemRepository
        {
            get
            {
                if (this._goalItemRepository == null)
                {
                    this._goalItemRepository = _repositoryBaseFactory.CreateGoalItemRepository(_context);
                }
                return _goalItemRepository;
            }
        }
        public IRepositoryBase<Day> DayRepository
        {
            get
            {
                if (this._dayRepository == null)
                {
                    this._dayRepository = _repositoryBaseFactory.CreateDayRepository(_context);
                }
                return _dayRepository;
            }
        }
        public IRepositoryBase<GoalItemsCreated> GoalItemsCreatedRepository
        {
            get
            {
                if (this._goalItemsCreatedRepository == null)
                {
                    this._goalItemsCreatedRepository = _repositoryBaseFactory.CreateGoalItemsCreatedRepository(_context);
                }
                return _goalItemsCreatedRepository;
            }
        }
        public IRepositoryBase<Settings> SettingsRepository
        {
            get
            {
                if (this._settingsRepository == null)
                {
                    this._settingsRepository = _repositoryBaseFactory.CreateSettingsRepository(_context);
                }
                return _settingsRepository;
            }
        }
        public void Save()
        {
            _context.Save();

            /*Dispose();
            _context = _contextFactory.CreateDbContext();

            _settingsRepository.SetContext(_context);
            _goalItemsCreatedRepository.SetContext(_context);
            _goalRepository.SetContext(_context);
            _dayRepository.SetContext(_context);
            _goalItemRepository.SetContext(_context);*/
        }
        public void Transaction(string tableName, Action work)
        {
            //_context.Transaction(tableName, work);
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
        public void Reload(object entity)
        {
            if (entity != null)
                _context.Reload(entity);
        }
    }
    public interface IRepositoryBaseFactory
    {
        RepositoryBase<Goal> CreateGoalRepository(IDbContext context);
        RepositoryBase<GoalItem> CreateGoalItemRepository(IDbContext context);
        RepositoryBase<Day> CreateDayRepository(IDbContext context);
        RepositoryBase<GoalItemsCreated> CreateGoalItemsCreatedRepository(IDbContext context);
        RepositoryBase<Settings> CreateSettingsRepository(IDbContext context);
    }
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork CreateUnitOfWork();
    }
}
