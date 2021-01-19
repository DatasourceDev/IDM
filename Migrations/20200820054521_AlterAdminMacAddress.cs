using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterAdminMacAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MacAddress1",
                table: "Admins",
                maxLength: 17,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MacAddress2",
                table: "Admins",
                maxLength: 17,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MacAddress3",
                table: "Admins",
                maxLength: 17,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MacAddress1",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "MacAddress2",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "MacAddress3",
                table: "Admins");
        }
    }
}
