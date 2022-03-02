using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Berry.Spider.EntityFrameworkCore.Migrations
{
    public partial class Add_TouTiao_220302 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Content",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    已采 = table.Column<int>(type: "INTEGER", nullable: false),
                    已发 = table.Column<int>(type: "INTEGER", nullable: false),
                    标题 = table.Column<string>(type: "TEXT", nullable: false),
                    内容 = table.Column<string>(type: "TEXT", nullable: false),
                    作者 = table.Column<string>(type: "TEXT", nullable: true),
                    时间 = table.Column<DateTime>(type: "TEXT", nullable: false),
                    出处 = table.Column<string>(type: "TEXT", nullable: false),
                    PageUrl = table.Column<string>(type: "TEXT", nullable: true),
                    关键字 = table.Column<string>(type: "TEXT", nullable: true),
                    tag = table.Column<string>(type: "TEXT", nullable: true),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Content", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Content");
        }
    }
}
