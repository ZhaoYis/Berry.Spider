using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Berry.Spider.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class add_IdentityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "spider_content_title",
                keyColumn: "ExtraProperties",
                keyValue: null,
                column: "ExtraProperties",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content_title",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_content_title",
                keyColumn: "ConcurrencyStamp",
                keyValue: null,
                column: "ConcurrencyStamp",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content_title",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_content_keyword",
                keyColumn: "ExtraProperties",
                keyValue: null,
                column: "ExtraProperties",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content_keyword",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_content_keyword",
                keyColumn: "ConcurrencyStamp",
                keyValue: null,
                column: "ConcurrencyStamp",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content_keyword",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_content_high_quality_qa",
                keyColumn: "ExtraProperties",
                keyValue: null,
                column: "ExtraProperties",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content_high_quality_qa",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_content_high_quality_qa",
                keyColumn: "ConcurrencyStamp",
                keyValue: null,
                column: "ConcurrencyStamp",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content_high_quality_qa",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                table: "spider_content_high_quality_qa",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_content_composition",
                keyColumn: "ExtraProperties",
                keyValue: null,
                column: "ExtraProperties",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content_composition",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_content_composition",
                keyColumn: "ConcurrencyStamp",
                keyValue: null,
                column: "ConcurrencyStamp",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content_composition",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                table: "spider_content_composition",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_content",
                keyColumn: "ExtraProperties",
                keyValue: null,
                column: "ExtraProperties",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_content",
                keyColumn: "ConcurrencyStamp",
                keyValue: null,
                column: "ConcurrencyStamp",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "IdentityId",
                table: "spider_content",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_basic_info",
                keyColumn: "ExtraProperties",
                keyValue: null,
                column: "ExtraProperties",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_basic_info",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "spider_basic_info",
                keyColumn: "ConcurrencyStamp",
                keyValue: null,
                column: "ConcurrencyStamp",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_basic_info",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "spider_content_high_quality_qa");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "spider_content_composition");

            migrationBuilder.DropColumn(
                name: "IdentityId",
                table: "spider_content");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content_title",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content_title",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content_keyword",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content_keyword",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content_high_quality_qa",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content_high_quality_qa",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content_composition",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content_composition",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_content",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_content",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraProperties",
                table: "spider_basic_info",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ConcurrencyStamp",
                table: "spider_basic_info",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
