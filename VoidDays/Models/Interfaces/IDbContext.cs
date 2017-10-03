using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
namespace VoidDays.Models.Interfaces
{
    public interface IDbContext
    {
        Database Database { get; }
        DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class;
        DbEntityEntry Entry(object entity);
        void Transaction(string tableName, Action work);
        void Save();
        void Dispose();
        void Reload(object entity);
    }
}