using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterGroupUser2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_table_group_user_table_group_group_id1",
                table: "table_group_user");

            migrationBuilder.DropForeignKey(
                name: "FK_table_group_user_Users_userID",
                table: "table_group_user");

            migrationBuilder.DropIndex(
                name: "IX_table_group_user_group_id1",
                table: "table_group_user");

            migrationBuilder.DropIndex(
                name: "IX_table_group_user_userID",
                table: "table_group_user");

            migrationBuilder.DropColumn(
                name: "group_id1",
                table: "table_group_user");

            migrationBuilder.DropColumn(
                name: "userID",
                table: "table_group_user");

            migrationBuilder.CreateIndex(
                name: "IX_table_group_user_group_id",
                table: "table_group_user",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_group_user_user_id",
                table: "table_group_user",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_table_group_user_table_group_group_id",
                table: "table_group_user",
                column: "group_id",
                principalTable: "table_group",
                principalColumn: "group_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_group_user_Users_user_id",
                table: "table_group_user",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_table_group_user_table_group_group_id",
                table: "table_group_user");

            migrationBuilder.DropForeignKey(
                name: "FK_table_group_user_Users_user_id",
                table: "table_group_user");

            migrationBuilder.DropIndex(
                name: "IX_table_group_user_group_id",
                table: "table_group_user");

            migrationBuilder.DropIndex(
                name: "IX_table_group_user_user_id",
                table: "table_group_user");

            migrationBuilder.AddColumn<int>(
                name: "group_id1",
                table: "table_group_user",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "userID",
                table: "table_group_user",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_table_group_user_group_id1",
                table: "table_group_user",
                column: "group_id1");

            migrationBuilder.CreateIndex(
                name: "IX_table_group_user_userID",
                table: "table_group_user",
                column: "userID");

            migrationBuilder.AddForeignKey(
                name: "FK_table_group_user_table_group_group_id1",
                table: "table_group_user",
                column: "group_id1",
                principalTable: "table_group",
                principalColumn: "group_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_group_user_Users_userID",
                table: "table_group_user",
                column: "userID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
