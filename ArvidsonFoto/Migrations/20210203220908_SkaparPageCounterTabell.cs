using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ArvidsonFoto.Migrations
{
    public partial class SkaparPageCounterTabell : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "menu_ENGtext",
                table: "tbl_menu");

            migrationBuilder.AlterColumn<int>(
                name: "menu_mainID",
                table: "tbl_menu",
                type: "int",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "menu_ID",
                table: "tbl_menu",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "image_ID",
                table: "tbl_images",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "tbl_gb",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_menu",
                table: "tbl_menu",
                column: "menu_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_images",
                table: "tbl_images",
                column: "image_ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tbl_gb",
                table: "tbl_gb",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "tbl_PageCounter",
                columns: table => new
                {
                    PageCounter_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PageCounter_Views = table.Column<int>(type: "int", nullable: false),
                    PageCounter_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PageCounter_MonthViewed = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PageCounter_LastShowDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_PageCounter", x => x.PageCounter_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_PageCounter");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_menu",
                table: "tbl_menu");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_images",
                table: "tbl_images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tbl_gb",
                table: "tbl_gb");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "tbl_gb");

            migrationBuilder.AlterColumn<short>(
                name: "menu_mainID",
                table: "tbl_menu",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "menu_ID",
                table: "tbl_menu",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "menu_ENGtext",
                table: "tbl_menu",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "image_ID",
                table: "tbl_images",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
