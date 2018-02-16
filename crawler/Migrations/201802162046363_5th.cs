namespace crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _5th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Filters", "OutName", c => c.String());
            AddColumn("dbo.Filters", "Location", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Filters", "Location");
            DropColumn("dbo.Filters", "OutName");
        }
    }
}
