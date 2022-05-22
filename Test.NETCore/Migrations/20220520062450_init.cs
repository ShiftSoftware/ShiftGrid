using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test.NETCore.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Types",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Types", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TestItems",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TypeId = table.Column<long>(type: "bigint", nullable: true),
                    ParentTestItemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestItems", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestItems_TestItems_ParentTestItemId",
                        column: x => x.ParentTestItemId,
                        principalTable: "TestItems",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestItems_Types_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Types",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestItems_ParentTestItemId",
                table: "TestItems",
                column: "ParentTestItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TestItems_TypeId",
                table: "TestItems",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestItems");

            migrationBuilder.DropTable(
                name: "Types");
        }
    }
}
