using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterSetupSMTP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SMTP_From",
                table: "Setups",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SMTP_Password",
                table: "Setups",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SMTP_Port",
                table: "Setups",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SMTP_SSL",
                table: "Setups",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SMTP_Server",
                table: "Setups",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SMTP_Username",
                table: "Setups",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SMTP_From",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "SMTP_Password",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "SMTP_Port",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "SMTP_SSL",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "SMTP_Server",
                table: "Setups");

            migrationBuilder.DropColumn(
                name: "SMTP_Username",
                table: "Setups");
        }
    }
}
