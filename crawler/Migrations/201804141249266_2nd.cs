namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2nd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Criteria", "ExpectedValue", c => c.String());
            DropColumn("dbo.Criteria", "OutName");
            DropColumn("dbo.Criteria", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Criteria", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Criteria", "OutName", c => c.String());
            DropColumn("dbo.Criteria", "ExpectedValue");
        }
    }
}
