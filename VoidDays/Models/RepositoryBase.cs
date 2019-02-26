using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models.Interfaces;
using System.Linq.Expressions;
using System.Data.Entity;
namespace VoidDays.Models
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        internal IDbContext _context;
        internal DbSet<TEntity> _dbSet;

        public RepositoryBase(IDbContext context)
        {
            this._context = context;

            this._dbSet = context.GetDbSet<TEntity>();
        }

        public RepositoryBase()
        {

        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", bool asNoTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                if (asNoTracking)
                    return orderBy(query).AsNoTracking().ToList();
                return orderBy(query).ToList();
            }
            else
            {
                if (asNoTracking)
                    return query.AsNoTracking().ToList();
                return query.ToList();
            }
        }
        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void SetContext(IDbContext context)
        {
            _context = context;
            this._dbSet = context.GetDbSet<TEntity>();
        }
    }
}

