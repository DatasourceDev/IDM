using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class Alterfacultyid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "system_faculty_id",
                table: "table_visual_fim_user",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "system_sub_office_id",
                table: "table_visual_fim_user",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "system_faculty_id",
                table: "table_visual_fim_user");

            migrationBuilder.DropColumn(
                name: "system_sub_office_id",
                table: "table_visual_fim_user");
        }
    }
}
