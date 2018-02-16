namespace crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class _3d : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sites",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BaseUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        Site_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);

            CreateTable(
                "dbo.Filters",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        XPath = c.String(),
                        Type = c.Int(nullable: false),
                        Category_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Category_Id);

            CreateTable(
                "dbo.Page",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Address = c.String(),
                        Text = c.String(),
                        SeeTime = c.DateTime(nullable: false),
                        IsSuccess = c.Boolean(nullable: false),
                        Site_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.Site_Id)
                .Index(t => t.Site_Id);

            DropTable("dbo.Data");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.Data",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Url = c.String(),
                        Text = c.String(),
                        CrawledSucess = c.Boolean(nullable: false),
                        Parsed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            DropForeignKey("dbo.Page", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Categories", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Filters", "Category_Id", "dbo.Categories");
            DropIndex("dbo.Page", new[] { "Site_Id" });
            DropIndex("dbo.Filters", new[] { "Category_Id" });
            DropIndex("dbo.Categories", new[] { "Site_Id" });
            DropTable("dbo.Page");
            DropTable("dbo.Filters");
            DropTable("dbo.Categories");
            DropTable("dbo.Sites");
        }
    }
}
