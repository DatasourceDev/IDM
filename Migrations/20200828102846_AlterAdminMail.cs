using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterAdminMail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "info",
                table: "Admins",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "spuOtherMails",
                table: "Admins",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "spuoffice365",
                table: "Admins",
                maxLength: 150,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "info",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "spuOtherMails",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "spuoffice365",
                table: "Admins");
        }
    }
}
