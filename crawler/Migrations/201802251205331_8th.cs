namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8th : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Filters", "Category_Id", "dbo.Categories");
            AddColumn("dbo.Filters", "Category_Id1", c => c.Long());
            AddColumn("dbo.Filters", "Category_Id2", c => c.Long());
            CreateIndex("dbo.Filters", "Category_Id1");
            CreateIndex("dbo.Filters", "Category_Id2");
            AddForeignKey("dbo.Filters", "Category_Id", "dbo.Categories", "Id");
            AddForeignKey("dbo.Filters", "Category_Id2", "dbo.Categories", "Id");
            AddForeignKey("dbo.Filters", "Category_Id1", "dbo.Categories", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Filters", "Category_Id1", "dbo.Categories");
            DropForeignKey("dbo.Filters", "Category_Id2", "dbo.Categories");
            DropForeignKey("dbo.Filters", "Category_Id", "dbo.Categories");
            DropIndex("dbo.Filters", new[] { "Category_Id2" });
            DropIndex("dbo.Filters", new[] { "Category_Id1" });
            DropColumn("dbo.Filters", "Category_Id2");
            DropColumn("dbo.Filters", "Category_Id1");
            AddForeignKey("dbo.Filters", "Category_Id", "dbo.Categories", "Id");
        }
    }
}
