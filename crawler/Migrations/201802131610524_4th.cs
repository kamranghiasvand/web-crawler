namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class _4th : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sites", "OutputFolder", c => c.String());
            AlterColumn("dbo.Categories", "Name", c => c.String(maxLength: 400, unicode: false));
            CreateIndex("dbo.Categories", "Name", unique: true);
        }

        public override void Down()
        {
            DropIndex("dbo.Categories", new[] { "Name" });
            AlterColumn("dbo.Categories", "Name", c => c.String());
            DropColumn("dbo.Sites", "OutputFolder");
        }
    }
}
