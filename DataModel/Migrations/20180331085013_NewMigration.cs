using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DataModel.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ListId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Des = table.Column<string>(nullable: true),
                    DueDate = table.Column<DateTimeOffset>(nullable: false),
                    Icon = table.Column<byte[]>(nullable: true),
                    IsCheck = table.Column<bool>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ListId);
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    ListId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Des = table.Column<string>(nullable: true),
                    DueDate = table.Column<DateTimeOffset>(nullable: false),
                    Icon = table.Column<byte[]>(nullable: true),
                    ListIndex = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.ListId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "State");
        }
    }
}
