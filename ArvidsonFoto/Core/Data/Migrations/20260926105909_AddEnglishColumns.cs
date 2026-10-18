using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArvidsonFoto.Core.Data.Migrations;

/// <inheritdoc />
public partial class _20260926105909_AddEnglishColumns : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<string>(
            name: "menu_URLtext_en",
            table: "tbl_menu",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "menu_text_en",
            table: "tbl_menu",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "image_description_en",
            table: "tbl_images",
            type: "nvarchar(150)",
            maxLength: 150,
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "GB_name_en",
            table: "tbl_gb",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: true);

        _ = migrationBuilder.AddColumn<string>(
            name: "GB_text_en",
            table: "tbl_gb",
            type: "nvarchar(max)",
            nullable: true);

    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "menu_URLtext_en",
            table: "tbl_menu");

        _ = migrationBuilder.DropColumn(
            name: "menu_text_en",
            table: "tbl_menu");

        _ = migrationBuilder.DropColumn(
            name: "image_description_en",
            table: "tbl_images");

        _ = migrationBuilder.DropColumn(
            name: "GB_name_en",
            table: "tbl_gb");

        _ = migrationBuilder.DropColumn(
            name: "GB_text_en",
            table: "tbl_gb");

    }
}
