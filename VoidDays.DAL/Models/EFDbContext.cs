using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.DAL.Models.Interfaces;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using VoidDays.DAL.Logging;
using System.Data;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Migrations.Model;
using VoidDays.DAL.Migrations;
using MySql.Data.EntityFramework;

namespace VoidDays.DAL.Models
{
    [DbConfigurationType(typeof(MySql.Data.EntityFramework.MySqlEFConfiguration))]
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

        public EFDbContext(): base("name=VoidDaysContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Database.Log += s => System.Diagnostics.Debug.WriteLine(s);
            Database.SetInitializer<EFDbContext>(null);
        }
        public EFDbContext(string connectionString)
            : base(connectionString)
            //:base("VoidDaysLiteContext")
        {            
            //this.Database.Log = s => Log.DBLog(s);
            this.Database.Log = s => Console.WriteLine(s);
            Database.SetInitializer<EFDbContext>(new MigrateDatabaseToLatestVersion<EFDbContext, Configuration>(true));
            //Database.SetInitializer<EFDbContext>(new CreateDatabaseIfNotExists<EFDbContext>());
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

    //class DatabaseConfiguration : DbConfiguration
    //{
    //    public DatabaseConfiguration()
    //    {
    //        SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
    //        SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
    //        SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
    //    }
    //}
    public class myMigrationSQLGenerator : MySqlMigrationSqlGenerator
    {
        private string TrimSchemaPrefix(string table)
        {
            if (table.StartsWith("dbo."))
                return table.Replace("dbo.", "");
            return table;
        }

        protected override MigrationStatement Generate(CreateIndexOperation op)
        {
            var u = new MigrationStatement();
            string unique = (op.IsUnique ? "UNIQUE" : ""), columns = "";
            foreach (var col in op.Columns)
            {
                columns += ($"`{col}` DESC{(op.Columns.IndexOf(col) < op.Columns.Count - 1 ? ", " : "")}");
            }
            u.Sql = $"CREATE {unique} INDEX `{op.Name}` ON `{TrimSchemaPrefix(op.Table)}` ({columns}) USING BTREE";
            return u;
        }
    }
}