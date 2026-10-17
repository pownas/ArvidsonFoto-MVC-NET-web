using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArvidsonFoto.Core.Data.Migrations;

/// <inheritdoc />
public partial class _20261017162636_AddTranslationReviewStatus : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "menu_URLtext_en_reviewed",
            table: "tbl_menu",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "menu_text_en_reviewed",
            table: "tbl_menu",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "image_description_en_reviewed",
            table: "tbl_images",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "GB_name_en_reviewed",
            table: "tbl_gb",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "GB_text_en_reviewed",
            table: "tbl_gb",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "menu_URLtext_en_reviewed",
            table: "tbl_menu");

        migrationBuilder.DropColumn(
            name: "menu_text_en_reviewed",
            table: "tbl_menu");

        migrationBuilder.DropColumn(
            name: "image_description_en_reviewed",
            table: "tbl_images");

        migrationBuilder.DropColumn(
            name: "GB_name_en_reviewed",
            table: "tbl_gb");

        migrationBuilder.DropColumn(
            name: "GB_text_en_reviewed",
            table: "tbl_gb");
    }
}
