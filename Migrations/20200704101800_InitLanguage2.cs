using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IDM.Migrations
{
    public partial class InitLanguage2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageCode = table.Column<int>(nullable: false),
                    LanguageKey = table.Column<string>(nullable: false),
                    LanguageValue = table.Column<string>(nullable: false),
                    Create_By = table.Column<string>(maxLength: 250, nullable: true),
                    Create_On = table.Column<DateTime>(nullable: true),
                    Update_By = table.Column<string>(maxLength: 250, nullable: true),
                    Update_On = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
