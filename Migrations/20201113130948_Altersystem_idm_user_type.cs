using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class Altersystem_idm_user_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "system_idm_user_type",
                table: "table_visual_fim_user",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "system_idm_user_type",
                table: "table_visual_fim_user");
        }
    }
}
