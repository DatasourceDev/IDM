using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class Altersystem_ou : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ad_created",
                table: "table_visual_fim_user",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ladp_created",
                table: "table_visual_fim_user",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "system_ou_lvl1",
                table: "table_visual_fim_user",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "system_ou_lvl2",
                table: "table_visual_fim_user",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "system_ou_lvl3",
                table: "table_visual_fim_user",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ad_created",
                table: "table_visual_fim_user");

            migrationBuilder.DropColumn(
                name: "ladp_created",
                table: "table_visual_fim_user");

            migrationBuilder.DropColumn(
                name: "system_ou_lvl1",
                table: "table_visual_fim_user");

            migrationBuilder.DropColumn(
                name: "system_ou_lvl2",
                table: "table_visual_fim_user");

            migrationBuilder.DropColumn(
                name: "system_ou_lvl3",
                table: "table_visual_fim_user");
        }
    }
}
