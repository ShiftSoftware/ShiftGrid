﻿namespace Test.NETFramework.MySQLMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TestItems",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Code = c.String(unicode: false),
                        Title = c.String(unicode: false),
                        Date = c.DateTime(precision: 0),
                        Price = c.Decimal(precision: 18, scale: 2),
                        TypeId = c.Long(),
                        ParentTestItemId = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.TestItems", t => t.ParentTestItemId)
                .ForeignKey("dbo.Types", t => t.TypeId)
                .Index(t => t.TypeId)
                .Index(t => t.ParentTestItemId);
            
            CreateTable(
                "dbo.Types",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TestItems", "TypeId", "dbo.Types");
            DropForeignKey("dbo.TestItems", "ParentTestItemId", "dbo.TestItems");
            DropIndex("dbo.TestItems", new[] { "ParentTestItemId" });
            DropIndex("dbo.TestItems", new[] { "TypeId" });
            DropTable("dbo.Types");
            DropTable("dbo.TestItems");
        }
    }
}
