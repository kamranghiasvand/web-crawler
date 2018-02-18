namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _6th : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Categories", new[] { "Site_Id" });
            CreateIndex("dbo.Categories", "site_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Categories", new[] { "site_Id" });
            CreateIndex("dbo.Categories", "Site_Id");
        }
    }
}
