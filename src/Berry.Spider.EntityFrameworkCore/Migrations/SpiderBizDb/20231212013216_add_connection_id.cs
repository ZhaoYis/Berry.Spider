using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Berry.Spider.EntityFrameworkCore.Migrations.SpiderBizDb
{
    /// <inheritdoc />
    public partial class add_connection_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectionId",
                table: "spider_serv_machine_info",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectionId",
                table: "spider_serv_machine_info");
        }
    }
}
