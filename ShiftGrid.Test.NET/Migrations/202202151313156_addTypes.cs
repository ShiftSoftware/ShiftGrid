namespace ShiftGrid.Test.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Types",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.TestItems", "TypeId", c => c.Long());
            CreateIndex("dbo.TestItems", "TypeId");
            AddForeignKey("dbo.TestItems", "TypeId", "dbo.Types", "ID");
            DropColumn("dbo.TestItems", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TestItems", "Type", c => c.Int());
            DropForeignKey("dbo.TestItems", "TypeId", "dbo.Types");
            DropIndex("dbo.TestItems", new[] { "TypeId" });
            DropColumn("dbo.TestItems", "TypeId");
            DropTable("dbo.Types");
        }
    }
}
