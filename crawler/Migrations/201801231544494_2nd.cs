namespace crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class _2nd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Data", "CrawledSucess", c => c.Boolean(nullable: false));
            AddColumn("dbo.Data", "Parsed", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Data", "Parsed");
            DropColumn("dbo.Data", "CrawledSucess");
        }
    }
}
