using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class InitLanguage3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Page",
                table: "Languages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Page",
                table: "Languages");
        }
    }
}
