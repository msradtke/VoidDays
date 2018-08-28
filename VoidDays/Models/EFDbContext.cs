using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models.Interfaces;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using VoidDays.Logging;
using System.Data;

namespace VoidDays.Models
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class EFDbContext : DbContext, IDbContext
    {
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalItem> GoalItems { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<GoalItemsCreated> GoalItemsCreated { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public ConnectionState ConnectionState { get; private set; }
        public EFDbContext()
            : base("VoidDays.Properties.Settings.VoidDaysConnectionString")
        {
            
            //this.Database.Log = s => Log.DBLog(s);
            this.Database.Log = s => Console.WriteLine(s);
            
            Database.SetInitializer<EFDbContext>(new CreateDatabaseIfNotExists<EFDbContext>());
            //Database.SetInitializer<EFDbContext>(new DropCreateDatabaseAlways<EFDbContext>());
            Database.Connection.StateChange += StateChangeHandler;
            this.Configuration.LazyLoadingEnabled = true;

        }
        static EFDbContext()
        {
            DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
        }
        private void StateChangeHandler(object sender, System.Data.StateChangeEventArgs e)
        {
            //this.ConnectionState = e.CurrentState;
            //throw new NotImplementedException();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GoalItem>().ToTable("goal_items");
            modelBuilder.Entity<Goal>().ToTable("goals");
            modelBuilder.Entity<Day>().ToTable("days");
            modelBuilder.Entity<GoalItemsCreated>().ToTable("goal_items_created");
            modelBuilder.Entity<Settings>().ToTable("settings");
            modelBuilder.Entity<GoalItem>().HasRequired(x => x.Goal);
            //modelBuilder.Entity<GoalItem>().HasOptional(x => x.Message);
        }
        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            return this.Set<TEntity>();
        }

        public void Save()
        {

            this.SaveChanges();
        }
        public void Reload(object entity)
        {
            this.Entry(entity).Reload();
        }
    }

    public interface IDbContextFactory
    {
        IDbContext CreateDbContext();
    }
}