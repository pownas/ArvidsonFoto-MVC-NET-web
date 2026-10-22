using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArvidsonFoto.Core.Data.Migrations;

/// <inheritdoc />
public partial class _20261017162636_AddTranslationReviewStatus : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.AddColumn<bool>(
            name: "menu_URLtext_en_reviewed",
            table: "tbl_menu",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "menu_text_en_reviewed",
            table: "tbl_menu",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "image_description_en_reviewed",
            table: "tbl_images",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "GB_name_en_reviewed",
            table: "tbl_gb",
            type: "bit",
            nullable: false,
            defaultValue: false);

        _ = migrationBuilder.AddColumn<bool>(
            name: "GB_text_en_reviewed",
            table: "tbl_gb",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.DropColumn(
            name: "menu_URLtext_en_reviewed",
            table: "tbl_menu");

        _ = migrationBuilder.DropColumn(
            name: "menu_text_en_reviewed",
            table: "tbl_menu");

        _ = migrationBuilder.DropColumn(
            name: "image_description_en_reviewed",
            table: "tbl_images");

        _ = migrationBuilder.DropColumn(
            name: "GB_name_en_reviewed",
            table: "tbl_gb");

        _ = migrationBuilder.DropColumn(
            name: "GB_text_en_reviewed",
            table: "tbl_gb");
    }
}
