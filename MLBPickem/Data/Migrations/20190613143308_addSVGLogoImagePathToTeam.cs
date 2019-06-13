using Microsoft.EntityFrameworkCore.Migrations;

namespace MLBPickem.Data.Migrations
{
    public partial class addSVGLogoImagePathToTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Team",
                nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Games_AwayTeamId",
            //    table: "Games",
            //    column: "AwayTeamId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Games_HomeTeamId",
            //    table: "Games",
            //    column: "HomeTeamId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Games_Team_AwayTeamId",
            //    table: "Games",
            //    column: "AwayTeamId",
            //    principalTable: "Team",
            //    principalColumn: "TeamId",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Games_Team_HomeTeamId",
            //    table: "Games",
            //    column: "HomeTeamId",
            //    principalTable: "Team",
            //    principalColumn: "TeamId",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Games_Team_AwayTeamId",
            //    table: "Games");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Games_Team_HomeTeamId",
            //    table: "Games");

            //migrationBuilder.DropIndex(
            //    name: "IX_Games_AwayTeamId",
            //    table: "Games");

            //migrationBuilder.DropIndex(
            //    name: "IX_Games_HomeTeamId",
            //    table: "Games");

            //migrationBuilder.DropColumn(
            //    name: "ImagePath",
            //    table: "Team");
        }
    }
}
