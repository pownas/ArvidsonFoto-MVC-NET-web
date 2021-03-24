using Microsoft.EntityFrameworkCore.Migrations;

namespace ArvidsonFoto.Migrations.ArvidsonFotoIdentity
{
    public partial class LagtTillShowAllLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowAllLogs",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowAllLogs",
                table: "AspNetUsers");
        }
    }
}
