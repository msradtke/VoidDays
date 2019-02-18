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
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using System.Data.Entity.Core.Common;
using SQLite.CodeFirst;

namespace VoidDays.Models
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    //[DbConfigurationType(typeof(DatabaseConfiguration))]
    public class EFDbContext : DbContext, IDbContext
    {
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalItem> GoalItems { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<GoalItemsCreated> GoalItemsCreated { get; set; }
        public DbSet<Settings> Settings { get; set; }

        public ConnectionState ConnectionState { get; private set; }
        string _connectionString;

        public EFDbContext(): base("VoidDays.Properties.Settings.VoidDaysConnectionString")
        {
        }
        public EFDbContext(string connectionString)
            : base(connectionString)
            //:base("VoidDaysLiteContext")
        {            
            //this.Database.Log = s => Log.DBLog(s);
            this.Database.Log = s => Console.WriteLine(s);

            Database.SetInitializer<EFDbContext>(new CreateDatabaseIfNotExists<EFDbContext>());
            //Database.SetInitializer<EFDbContext>(new MigrateDatabaseToLatestVersion<>
            //Database.SetInitializer<EFDbContext>(new DropCreateDatabaseAlways<EFDbContext>());
            this.Configuration.LazyLoadingEnabled = true;

        }
        static EFDbContext()
        {
            //DbConfiguration.SetConfiguration(new DatabaseConfiguration());
        }
        private void StateChangeHandler(object sender, System.Data.StateChangeEventArgs e)
        {
            this.ConnectionState = e.CurrentState;
            //throw new NotImplementedException();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //Database.SetInitializer<EFDbContext>(new SqliteCreateDatabaseIfNotExists<EFDbContext>(modelBuilder));
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
        IDbContext CreateDbContext(string connectionString);
    }

    class DatabaseConfiguration : DbConfiguration
    {
        public DatabaseConfiguration()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
}