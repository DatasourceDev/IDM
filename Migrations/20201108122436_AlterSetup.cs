using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "OpenSMS",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "SMSGatewayUrl",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "SMSPassword",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "SMSSender",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "SMSUsername",
                table: "Setups");

            migrationBuilder.AddColumn<string>(
                name: "LDAPBase",
                table: "Setups",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LDAPHost",
                table: "Setups",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LDAPPassword",
                table: "Setups",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LDAPPort",
                table: "Setups",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LDAPUsername",
                table: "Setups",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LDAPBase",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "LDAPHost",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "LDAPPassword",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "LDAPPort",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "LDAPUsername",
                table: "Setups");

            migrationBuilder.AddColumn<int>(
                name: "MacAddressNumShowAdmin",
                table: "Setups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MacAddressNumShowGuest",
                table: "Setups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MacAddressNumShowStaff",
                table: "Setups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MacAddressNumShowStudent",
                table: "Setups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OpenSMS",
                table: "Setups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SMSGatewayUrl",
                table: "Setups",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SMSPassword",
                table: "Setups",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SMSSender",
                table: "Setups",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SMSUsername",
                table: "Setups",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
