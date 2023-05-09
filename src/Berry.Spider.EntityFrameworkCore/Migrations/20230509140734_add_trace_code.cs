using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Berry.Spider.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class add_trace_code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TraceCode",
                table: "spider_content_title",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TraceCode",
                table: "spider_content_keyword",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TraceCode",
                table: "spider_content_high_quality_qa",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TraceCode",
                table: "spider_content_composition",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TraceCode",
                table: "spider_content",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TraceCode",
                table: "spider_content_title");

            migrationBuilder.DropColumn(
                name: "TraceCode",
                table: "spider_content_keyword");

            migrationBuilder.DropColumn(
                name: "TraceCode",
                table: "spider_content_high_quality_qa");

            migrationBuilder.DropColumn(
                name: "TraceCode",
                table: "spider_content_composition");

            migrationBuilder.DropColumn(
                name: "TraceCode",
                table: "spider_content");
        }
    }
}
