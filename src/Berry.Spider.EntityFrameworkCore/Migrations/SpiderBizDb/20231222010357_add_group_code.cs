using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Berry.Spider.EntityFrameworkCore.Migrations.SpiderBizDb
{
    /// <inheritdoc />
    public partial class add_group_code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupCode",
                table: "spider_serv_machine_info",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_spider_serv_machine_info_GroupCode",
                table: "spider_serv_machine_info",
                column: "GroupCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_spider_serv_machine_info_GroupCode",
                table: "spider_serv_machine_info");

            migrationBuilder.DropColumn(
                name: "GroupCode",
                table: "spider_serv_machine_info");
        }
    }
}
