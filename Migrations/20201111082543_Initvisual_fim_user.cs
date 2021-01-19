using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class Initvisual_fim_user : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminRoles_Admins_AdminID",
                table: "AdminRoles");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_AdminRoles_AdminID",
                table: "AdminRoles");

            migrationBuilder.DropColumn(
                name: "AdminID",
                table: "AdminRoles");

            migrationBuilder.AddColumn<int>(
                name: "visual_fim_user_id",
                table: "AdminRoles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "visual_fim_userid",
                table: "AdminRoles",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "table_visual_fim_user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    basic_dn = table.Column<string>(nullable: true),
                    basic_cn = table.Column<string>(nullable: true),
                    basic_sn = table.Column<string>(nullable: true),
                    basic_displayname = table.Column<string>(nullable: true),
                    basic_mail = table.Column<string>(nullable: true),
                    basic_mobile = table.Column<string>(nullable: true),
                    basic_uid = table.Column<string>(nullable: true),
                    basic_givenname = table.Column<string>(nullable: true),
                    basic_userprincipalname = table.Column<string>(nullable: true),
                    basic_telephonenumber = table.Column<string>(nullable: true),
                    basic_userPassword = table.Column<string>(nullable: true),
                    cu_nsaccountlock = table.Column<string>(nullable: true),
                    cu_mailhost = table.Column<string>(nullable: true),
                    cu_mailRoutingAddress = table.Column<string>(nullable: true),
                    cu_maildrop = table.Column<string>(nullable: true),
                    cu_mailacceptinggeneralid = table.Column<string>(nullable: true),
                    cu_pwdchangedby = table.Column<string>(nullable: true),
                    cu_pwdchangedloc = table.Column<string>(nullable: true),
                    cu_gecos = table.Column<string>(nullable: true),
                    cu_thcn = table.Column<string>(nullable: true),
                    cu_thsn = table.Column<string>(nullable: true),
                    cu_CUexpire = table.Column<string>(nullable: true),
                    cu_jobcode = table.Column<string>(nullable: true),
                    cu_pplid = table.Column<string>(nullable: true),
                    cu_sce_package = table.Column<string>(nullable: true),
                    unix_gidNumber = table.Column<string>(nullable: true),
                    unix_uidNumber = table.Column<string>(nullable: true),
                    unix_homeDirectory = table.Column<string>(nullable: true),
                    unix_loginShell = table.Column<string>(nullable: true),
                    unix_inetCOS = table.Column<string>(nullable: true),
                    mail_miWmprefFullName = table.Column<string>(nullable: true),
                    mail_miWmprefEmailAddress = table.Column<string>(nullable: true),
                    mail_miWmprefReplyOption = table.Column<string>(nullable: true),
                    mail_miWmprefTimezone = table.Column<string>(nullable: true),
                    mail_miWmprefCharset = table.Column<string>(nullable: true),
                    system_question1 = table.Column<string>(nullable: true),
                    system_answer1 = table.Column<string>(nullable: true),
                    system_question2 = table.Column<string>(nullable: true),
                    system_answer2 = table.Column<string>(nullable: true),
                    system_question3 = table.Column<string>(nullable: true),
                    system_answer3 = table.Column<string>(nullable: true),
                    system_create_by_uid = table.Column<string>(nullable: true),
                    system_modify_by_uid = table.Column<string>(nullable: true),
                    system_last_accessed_by_uid = table.Column<string>(nullable: true),
                    system_create_date = table.Column<string>(nullable: true),
                    system_modify_date = table.Column<string>(nullable: true),
                    system_last_accessed_date = table.Column<string>(nullable: true),
                    system_waiting_time_for_access = table.Column<string>(nullable: true),
                    system_temporary_user_expire_date_counter = table.Column<string>(nullable: true),
                    system_org = table.Column<string>(nullable: true),
                    system_enable_password_forgot = table.Column<string>(nullable: true),
                    internetaccess = table.Column<string>(nullable: true),
                    netcastaccess = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_visual_fim_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_visual_fim_user_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminRoles_visual_fim_userid",
                table: "AdminRoles",
                column: "visual_fim_userid");

            migrationBuilder.CreateIndex(
                name: "IX_table_visual_fim_user_UserID",
                table: "table_visual_fim_user",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminRoles_table_visual_fim_user_visual_fim_userid",
                table: "AdminRoles",
                column: "visual_fim_userid",
                principalTable: "table_visual_fim_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminRoles_table_visual_fim_user_visual_fim_userid",
                table: "AdminRoles");

            migrationBuilder.DropTable(
                name: "table_visual_fim_user");

            migrationBuilder.DropIndex(
                name: "IX_AdminRoles_visual_fim_userid",
                table: "AdminRoles");

            migrationBuilder.DropColumn(
                name: "visual_fim_user_id",
                table: "AdminRoles");

            migrationBuilder.DropColumn(
                name: "visual_fim_userid",
                table: "AdminRoles");

            migrationBuilder.AddColumn<int>(
                name: "AdminID",
                table: "AdminRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminCode = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Create_By = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    MacAddress1 = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true),
                    MacAddress2 = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true),
                    MacAddress3 = table.Column<string>(type: "nvarchar(17)", maxLength: 17, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Update_By = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    info = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    spuOtherMails = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    spuoffice365 = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Admins_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminRoles_AdminID",
                table: "AdminRoles",
                column: "AdminID");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserID",
                table: "Admins",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminRoles_Admins_AdminID",
                table: "AdminRoles",
                column: "AdminID",
                principalTable: "Admins",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
