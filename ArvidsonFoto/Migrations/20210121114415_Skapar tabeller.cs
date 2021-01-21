using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ArvidsonFoto.Migrations
{
    public partial class Skapartabeller : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_admin",
                columns: table => new
                {
                    admin_ID = table.Column<int>(type: "int", nullable: false),
                    admin_user = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    admin_pass = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    admin_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    admin_mail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    admin_lastonline = table.Column<DateTime>(type: "smalldatetime", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "tbl_gb",
                columns: table => new
                {
                    GB_ID = table.Column<int>(type: "int", nullable: false),
                    GB_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GB_email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GB_homepage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GB_text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GB_date = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    GB_IP = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "tbl_images",
                columns: table => new
                {
                    image_ID = table.Column<int>(type: "int", nullable: false),
                    image_huvudfamilj = table.Column<int>(type: "int", nullable: true),
                    image_familj = table.Column<int>(type: "int", nullable: true),
                    image_art = table.Column<int>(type: "int", nullable: false),
                    image_URL = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    image_date = table.Column<DateTime>(type: "smalldatetime", nullable: true),
                    image_description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    image_update = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "tbl_menu",
                columns: table => new
                {
                    menu_ID = table.Column<int>(type: "int", nullable: false),
                    menu_mainID = table.Column<short>(type: "smallint", nullable: true),
                    menu_text = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    menu_URLtext = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    menu_ENGtext = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    menu_lastshowdate = table.Column<DateTime>(type: "datetime", nullable: true),
                    menu_pagecounter = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_admin");

            migrationBuilder.DropTable(
                name: "tbl_gb");

            migrationBuilder.DropTable(
                name: "tbl_images");

            migrationBuilder.DropTable(
                name: "tbl_menu");
        }
    }
}
