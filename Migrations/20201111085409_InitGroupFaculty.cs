using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class InitGroupFaculty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "system_waiting_time_for_access",
                table: "table_visual_fim_user",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "system_temporary_user_expire_date_counter",
                table: "table_visual_fim_user",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "system_modify_date",
                table: "table_visual_fim_user",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "system_last_accessed_date",
                table: "table_visual_fim_user",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "system_enable_password_forgot",
                table: "table_visual_fim_user",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "system_create_date",
                table: "table_visual_fim_user",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "netcastaccess",
                table: "table_visual_fim_user",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "internetaccess",
                table: "table_visual_fim_user",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "table_cu_faculty",
                columns: table => new
                {
                    faculty_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    faculty_name = table.Column<string>(nullable: true),
                    faculty_name_eng = table.Column<string>(nullable: true),
                    faculty_shot_name = table.Column<string>(nullable: true),
                    faculty_distinguish_name_staff = table.Column<string>(nullable: true),
                    faculty_distinguish_name_student = table.Column<string>(nullable: true),
                    faculty_distinguish_name_outsider = table.Column<string>(nullable: true),
                    faculty_distinguish_name_affiliate = table.Column<string>(nullable: true),
                    faculty_telephonenumber = table.Column<string>(nullable: true),
                    create_date = table.Column<DateTime>(nullable: true),
                    create_by_username = table.Column<string>(nullable: true),
                    modified_date = table.Column<DateTime>(nullable: true),
                    modified_by_username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_cu_faculty", x => x.faculty_id);
                });

            migrationBuilder.CreateTable(
                name: "table_cu_faculty_level2",
                columns: table => new
                {
                    sub_office_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sub_office_name = table.Column<string>(nullable: true),
                    sub_office_name_eng = table.Column<string>(nullable: true),
                    sub_office_shot_name = table.Column<string>(nullable: true),
                    sub_office_telephonenumber = table.Column<string>(nullable: true),
                    faculty_id = table.Column<int>(nullable: true),
                    faculty_name = table.Column<string>(nullable: true),
                    create_by_username = table.Column<string>(nullable: true),
                    create_date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_cu_faculty_level2", x => x.sub_office_id);
                });

            migrationBuilder.CreateTable(
                name: "table_group",
                columns: table => new
                {
                    group_id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_name = table.Column<string>(nullable: true),
                    group_username_list = table.Column<string>(nullable: true),
                    group_manage_distinguish_name_list = table.Column<int>(nullable: true),
                    group_priority = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_group", x => x.group_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "table_cu_faculty");

            migrationBuilder.DropTable(
                name: "table_cu_faculty_level2");

            migrationBuilder.DropTable(
                name: "table_group");

            migrationBuilder.AlterColumn<string>(
                name: "system_waiting_time_for_access",
                table: "table_visual_fim_user",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "system_temporary_user_expire_date_counter",
                table: "table_visual_fim_user",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "system_modify_date",
                table: "table_visual_fim_user",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "system_last_accessed_date",
                table: "table_visual_fim_user",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "system_enable_password_forgot",
                table: "table_visual_fim_user",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "system_create_date",
                table: "table_visual_fim_user",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "netcastaccess",
                table: "table_visual_fim_user",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "internetaccess",
                table: "table_visual_fim_user",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
