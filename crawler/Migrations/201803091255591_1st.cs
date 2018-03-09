namespace Crawler.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1st : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 400, unicode: false),
                        site_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sites", t => t.site_Id)
                .Index(t => t.Name, unique: true)
                .Index(t => t.site_Id);
            
            CreateTable(
                "dbo.Criteria",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Location = c.Int(nullable: false),
                        Name = c.String(),
                        OutName = c.String(),
                        Type = c.Int(nullable: false),
                        Selector = c.String(),
                        Category_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Filters",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Location = c.Int(nullable: false),
                        Name = c.String(),
                        OutName = c.String(),
                        Type = c.Int(nullable: false),
                        Selector = c.String(),
                        Category_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.Category_Id)
                .Index(t => t.Category_Id);
            
            CreateTable(
                "dbo.Sites",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BaseUrl = c.String(),
                        Name = c.String(),
                        OutputFolder = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Page", "Site_Id", "dbo.Sites");
            DropForeignKey("dbo.Categories", "site_Id", "dbo.Sites");
            DropForeignKey("dbo.Filters", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Criteria", "Category_Id", "dbo.Categories");
            DropIndex("dbo.Page", new[] { "Site_Id" });
            DropIndex("dbo.Filters", new[] { "Category_Id" });
            DropIndex("dbo.Criteria", new[] { "Category_Id" });
            DropIndex("dbo.Categories", new[] { "site_Id" });
            DropIndex("dbo.Categories", new[] { "Name" });
            DropTable("dbo.Page");
            DropTable("dbo.Sites");
            DropTable("dbo.Filters");
            DropTable("dbo.Criteria");
            DropTable("dbo.Categories");
        }
    }
}
