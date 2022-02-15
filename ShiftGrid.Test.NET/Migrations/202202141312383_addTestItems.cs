namespace ShiftGrid.Test.NET.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTestItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestItems",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Code = c.String(),
                        Title = c.String(),
                        Date = c.DateTime(),
                        Price = c.Decimal(precision: 18, scale: 2),
                        Type = c.Int(),
                        ParentTestItemId = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TestItems", t => t.ParentTestItemId)
                .Index(t => t.ParentTestItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestItems", "ParentTestItemId", "dbo.TestItems");
            DropIndex("dbo.TestItems", new[] { "ParentTestItemId" });
            DropTable("dbo.TestItems");
        }
    }
}
