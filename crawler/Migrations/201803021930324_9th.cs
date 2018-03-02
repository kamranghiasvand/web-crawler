namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _9th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Filters", "Selector", c => c.String());
            DropColumn("dbo.Filters", "XPath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Filters", "XPath", c => c.String());
            DropColumn("dbo.Filters", "Selector");
        }
    }
}
