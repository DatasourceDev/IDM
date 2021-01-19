using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterOUDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OUDescription",
                table: "OUs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OUDescription",
                table: "OUs");
        }
    }
}
