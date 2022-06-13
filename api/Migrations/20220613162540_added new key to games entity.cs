using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    public partial class addednewkeytogamesentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Games_AppId",
                table: "Games");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Games_AppId",
                table: "Games",
                column: "AppId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Games_AppId",
                table: "Games");

            migrationBuilder.CreateIndex(
                name: "IX_Games_AppId",
                table: "Games",
                column: "AppId");
        }
    }
}
