using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models.Interfaces;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
namespace VoidDays.Models
{
    public class EFDbContext : DbContext, IDbContext
    {

        public EFDbContext()
            :base ("VoidDaysContext")
        {
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
            Database.SetInitializer<EFDbContext>(null);
            this.Configuration.LazyLoadingEnabled = true;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Goal>().ToTable("goals");

            modelBuilder.Entity<Goal>().HasRequired(x => x.Title);
        }
        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return this.Set<TEntity>();
        }

        public void Save()
        {
            this.SaveChanges();
        }
    }
}
