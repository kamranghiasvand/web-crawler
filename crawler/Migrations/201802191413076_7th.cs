namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class _7th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "Name", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Sites", "Name");
        }
    }
}
