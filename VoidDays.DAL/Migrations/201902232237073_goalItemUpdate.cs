namespace VoidDays.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class goalItemUpdate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.goal_items", "CompletedDate");
            DropColumn("dbo.goal_items", "DeletedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.goal_items", "DeletedDate", c => c.DateTime(nullable: false, precision: 0));
            AddColumn("dbo.goal_items", "CompletedDate", c => c.DateTime(nullable: false, precision: 0));
        }
    }
}
