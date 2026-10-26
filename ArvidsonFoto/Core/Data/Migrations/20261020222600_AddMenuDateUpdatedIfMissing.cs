using ArvidsonFoto.Core.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArvidsonFoto.Core.Data.Migrations;

[DbContext(typeof(ArvidsonFotoCoreDbContext))]
[Migration("20261020222600_AddMenuDateUpdatedIfMissing")]
public partial class AddMenuDateUpdatedIfMissing : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF COL_LENGTH(N'dbo.tbl_menu', N'menu_dateUpdated') IS NULL
                ALTER TABLE [dbo].[tbl_menu] ADD [menu_dateUpdated] datetime NULL;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}