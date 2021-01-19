using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterMacAddressNumShow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MacAddressNumShowAdmin",
                table: "Setups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MacAddressNumShowGuest",
                table: "Setups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MacAddressNumShowStaff",
                table: "Setups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MacAddressNumShowStudent",
                table: "Setups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MacAddressNumShowAdmin",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "MacAddressNumShowGuest",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "MacAddressNumShowStaff",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "MacAddressNumShowStudent",
                table: "Setups");
        }
    }
}
