using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class AlterGroupUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminRoles");

            migrationBuilder.CreateTable(
                name: "table_group_user",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    group_id = table.Column<int>(nullable: false),
                    user_id = table.Column<int>(nullable: false),
                    group_id1 = table.Column<int>(nullable: true),
                    userID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_group_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_group_user_table_group_group_id1",
                        column: x => x.group_id1,
                        principalTable: "table_group",
                        principalColumn: "group_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_table_group_user_Users_userID",
                        column: x => x.userID,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_table_group_user_group_id1",
                table: "table_group_user",
                column: "group_id1");

            migrationBuilder.CreateIndex(
                name: "IX_table_group_user_userID",
                table: "table_group_user",
                column: "userID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "table_group_user");

            migrationBuilder.CreateTable(
                name: "AdminRoles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    visual_fim_user_id = table.Column<int>(type: "int", nullable: false),
                    visual_fim_userid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminRoles", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AdminRoles_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdminRoles_table_visual_fim_user_visual_fim_userid",
                        column: x => x.visual_fim_userid,
                        principalTable: "table_visual_fim_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminRoles_RoleID",
                table: "AdminRoles",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_AdminRoles_visual_fim_userid",
                table: "AdminRoles",
                column: "visual_fim_userid");
        }
    }
}
