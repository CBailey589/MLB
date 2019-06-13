using Microsoft.EntityFrameworkCore.Migrations;

namespace MLBPickem.Data.Migrations
{
    public partial class FixedUserGameAsTeamTypeErrorInDBContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Games_Team_AwayTeamId",
            //    table: "Games");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Games_Team_HomeTeamId",
            //    table: "Games");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserGame_Games_GameId",
            //    table: "UserGame");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserGame_AspNetUsers_UserId",
            //    table: "UserGame");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGame",
                table: "UserGame");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Team",
                table: "Team");

            migrationBuilder.RenameTable(
                name: "UserGame",
                newName: "UserGames");

            migrationBuilder.RenameTable(
                name: "Team",
                newName: "Teams");

            migrationBuilder.RenameIndex(
                name: "IX_UserGame_UserId",
                table: "UserGames",
                newName: "IX_UserGames_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGame_GameId",
                table: "UserGames",
                newName: "IX_UserGames_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGames",
                table: "UserGames",
                column: "UserGameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teams",
                table: "Teams",
                column: "TeamId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Games_Teams_AwayTeamId",
            //    table: "Games",
            //    column: "AwayTeamId",
            //    principalTable: "Teams",
            //    principalColumn: "TeamId",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Games_Teams_HomeTeamId",
            //    table: "Games",
            //    column: "HomeTeamId",
            //    principalTable: "Teams",
            //    principalColumn: "TeamId",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserGames_Games_GameId",
            //    table: "UserGames",
            //    column: "GameId",
            //    principalTable: "Games",
            //    principalColumn: "GameId",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserGames_AspNetUsers_UserId",
            //    table: "UserGames",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Games_Teams_AwayTeamId",
            //    table: "Games");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Games_Teams_HomeTeamId",
            //    table: "Games");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserGames_Games_GameId",
            //    table: "UserGames");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserGames_AspNetUsers_UserId",
            //    table: "UserGames");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_UserGames",
            //    table: "UserGames");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_Teams",
            //    table: "Teams");

            //migrationBuilder.RenameTable(
            //    name: "UserGames",
            //    newName: "UserGame");

            //migrationBuilder.RenameTable(
            //    name: "Teams",
            //    newName: "Team");

            //migrationBuilder.RenameIndex(
            //    name: "IX_UserGames_UserId",
            //    table: "UserGame",
            //    newName: "IX_UserGame_UserId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_UserGames_GameId",
            //    table: "UserGame",
            //    newName: "IX_UserGame_GameId");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_UserGame",
            //    table: "UserGame",
            //    column: "UserGameId");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_Team",
            //    table: "Team",
            //    column: "TeamId");

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

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserGame_Games_GameId",
            //    table: "UserGame",
            //    column: "GameId",
            //    principalTable: "Games",
            //    principalColumn: "GameId",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserGame_AspNetUsers_UserId",
            //    table: "UserGame",
            //    column: "UserId",
            //    principalTable: "AspNetUsers",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
