﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidDays.Models.Interfaces;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using VoidDays.Logging;

namespace VoidDays.Models
{
    public class EFDbContext : DbContext, IDbContext
    {
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalItem> GoalItems { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<GoalItemsCreated> GoalItemsCreated { get; set; }
        public DbSet<Settings> Settings { get; set; }

        public EFDbContext()
            :base ("VoidDaysContext")
        {

            //this.Database.Log = s => Log.DBLog(s);
            this.Database.Log = s => Console.WriteLine(s);
            Database.SetInitializer<EFDbContext>(null);
            this.Configuration.LazyLoadingEnabled = true;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GoalItem>().ToTable("goal_items");
            modelBuilder.Entity<Goal>().ToTable("goals");
            modelBuilder.Entity<Day>().ToTable("days");
            modelBuilder.Entity<GoalItemsCreated>().ToTable("goal_items_created");
            modelBuilder.Entity<Settings>().ToTable("settings");
            modelBuilder.Entity<GoalItem>().HasRequired(x => x.Goal);            
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
