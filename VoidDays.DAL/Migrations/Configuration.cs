namespace VoidDays.DAL.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using VoidDays.DAL.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<VoidDays.DAL.Models.EFDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            SetSqlGenerator("MySql.Data.MySqlClient", new myMigrationSQLGenerator());
        }

        protected override void Seed(VoidDays.DAL.Models.EFDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
