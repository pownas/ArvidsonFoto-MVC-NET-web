using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArvidsonFoto.Core.Data.Migrations;

/// <inheritdoc />
public partial class _20261018210222_AddGameSelectableFlags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "is_game_selectable",
            table: "tbl_menu",
            type: "bit",
            nullable: false,
            defaultValue: true);

        migrationBuilder.AddColumn<bool>(
            name: "is_game_selectable",
            table: "tbl_images",
            type: "bit",
            nullable: false,
            defaultValue: true);

        migrationBuilder.Sql("""
            DECLARE @excludedCategories TABLE (menu_ID int PRIMARY KEY);

            WITH excludedCategories AS (
                SELECT menu_ID
                FROM tbl_menu
                WHERE COALESCE(menu_mainID, 0) = 0
                    AND (
                        menu_text IN (N'Årstider', N'Resor', N'Landskap')
                        OR menu_text_en IN (N'Seasons', N'Travel', N'Landscapes')
                        OR menu_URLtext IN (N'Arstider', N'Resor', N'Landskap')
                        OR menu_URLtext_en IN (N'seasons', N'travel', N'landscapes')
                    )

                UNION ALL

                SELECT child.menu_ID
                FROM tbl_menu AS child
                INNER JOIN excludedCategories AS parent
                    ON child.menu_mainID = parent.menu_ID
            )
            INSERT INTO @excludedCategories (menu_ID)
            SELECT menu_ID
            FROM excludedCategories
            OPTION (MAXRECURSION 32767);

            UPDATE tbl_menu
            SET is_game_selectable = 0
            WHERE menu_ID IN (SELECT menu_ID FROM @excludedCategories);

            UPDATE tbl_images
            SET is_game_selectable = 0
            WHERE image_art IN (SELECT menu_ID FROM @excludedCategories)
                OR image_familj IN (SELECT menu_ID FROM @excludedCategories)
                OR image_huvudfamilj IN (SELECT menu_ID FROM @excludedCategories);
            """);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "is_game_selectable",
            table: "tbl_menu");

        migrationBuilder.DropColumn(
            name: "is_game_selectable",
            table: "tbl_images");
    }
}
