namespace VoidDays.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSnapshot : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.days",
                c => new
                    {
                        day_id = c.Int(nullable: false, identity: true),
                        day_number = c.Int(nullable: false),
                        start = c.DateTime(nullable: false, precision: 0),
                        end = c.DateTime(nullable: false, precision: 0),
                        is_active = c.Boolean(nullable: false),
                        is_void = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.day_id);
            
            CreateTable(
                "dbo.goal_items",
                c => new
                    {
                        goal_item_id = c.Int(nullable: false, identity: true),
                        Title = c.String(unicode: false),
                        message = c.String(unicode: false),
                        created = c.DateTime(nullable: false, precision: 0),
                        is_complete = c.Boolean(nullable: false),
                        is_void = c.Boolean(nullable: false),
                        goal_id = c.Int(nullable: false),
                        day_number = c.Int(nullable: false),
                        complete_message = c.String(unicode: false),
                        satisfy_scale = c.Int(nullable: false),
                        CompletedDate = c.DateTime(nullable: false, precision: 0),
                        DeletedDate = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.goal_item_id)
                .ForeignKey("dbo.goals", t => t.goal_id, cascadeDelete: true)
                .Index(t => t.goal_id);
            
            CreateTable(
                "dbo.goals",
                c => new
                    {
                        goal_id = c.Int(nullable: false, identity: true),
                        message = c.String(unicode: false),
                        created = c.DateTime(nullable: false, precision: 0),
                        title = c.String(unicode: false),
                        is_active = c.Boolean(nullable: false),
                        is_complete = c.Boolean(nullable: false),
                        is_deleted = c.Boolean(nullable: false),
                        delete_date = c.DateTime(nullable: false, precision: 0),
                        complete_date = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.goal_id);
            
            CreateTable(
                "dbo.goal_items_created",
                c => new
                    {
                        goal_items_created_id = c.Int(nullable: false, identity: true),
                        day_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.goal_items_created_id)
                .ForeignKey("dbo.days", t => t.day_id, cascadeDelete: true)
                .Index(t => t.day_id);
            
            CreateTable(
                "dbo.settings",
                c => new
                    {
                        settings_id = c.Int(nullable: false, identity: true),
                        start_time = c.DateTime(nullable: false, precision: 0),
                        end_time = c.DateTime(nullable: false, precision: 0),
                        start_day = c.DateTime(nullable: false, precision: 0),
                        is_updating = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.settings_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.goal_items_created", "day_id", "dbo.days");
            DropForeignKey("dbo.goal_items", "goal_id", "dbo.goals");
            DropIndex("dbo.goal_items_created", new[] { "day_id" });
            DropIndex("dbo.goal_items", new[] { "goal_id" });
            DropTable("dbo.settings");
            DropTable("dbo.goal_items_created");
            DropTable("dbo.goals");
            DropTable("dbo.goal_items");
            DropTable("dbo.days");
        }
    }
}
