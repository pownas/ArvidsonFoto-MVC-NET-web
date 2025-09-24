using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArvidsonFoto.Migrations
{
    public partial class AddNewsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_news",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    news_ID = table.Column<int>(type: "int", nullable: false),
                    news_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    news_content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    news_author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    news_created = table.Column<DateTime>(type: "datetime", nullable: true),
                    news_updated = table.Column<DateTime>(type: "datetime", nullable: true),
                    news_published = table.Column<bool>(type: "bit", nullable: false),
                    news_summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_news", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_news");
        }
    }
}