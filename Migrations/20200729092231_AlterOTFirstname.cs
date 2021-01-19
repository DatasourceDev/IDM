using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterOTFirstname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "OTPs",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "OTPs",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "OTPs");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "OTPs");
        }
    }
}
