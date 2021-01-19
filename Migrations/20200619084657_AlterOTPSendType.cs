using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterOTPSendType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OTPs",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SendMessageType",
                table: "OTPs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "OTPs");

            migrationBuilder.DropColumn(
                name: "SendMessageType",
                table: "OTPs");
        }
    }
}
