using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterOUDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Create_By",
                table: "OUs",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Create_On",
                table: "OUs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Update_By",
                table: "OUs",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Update_On",
                table: "OUs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Create_By",
                table: "OUs");

            migrationBuilder.DropColumn(
                name: "Create_On",
                table: "OUs");

            migrationBuilder.DropColumn(
                name: "Update_By",
                table: "OUs");

            migrationBuilder.DropColumn(
                name: "Update_On",
                table: "OUs");
        }
    }
}
