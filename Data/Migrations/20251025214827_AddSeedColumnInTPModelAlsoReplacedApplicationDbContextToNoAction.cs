using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TournamentManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedColumnInTPModelAlsoReplacedApplicationDbContextToNoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_AspNetUsers_PlayerAId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_AspNetUsers_PlayerBId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_AspNetUsers_RefereeId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_AspNetUsers_WinnerId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Matches_NextMatchId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Tournaments_TournamentId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerProfiles_AspNetUsers_ApplicationUserId",
                table: "PlayerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayers_AspNetUsers_PlayerId",
                table: "TournamentPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayers_Tournaments_TournamentId",
                table: "TournamentPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_AspNetUsers_OrganizerId",
                table: "Tournaments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "597ac129-4fa0-4645-ad53-687245e84813");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73b5078d-0f91-4731-b673-6111845c55e6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e2fcb191-58d6-4823-98b1-e00fbbc36ba8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f2905c1f-6a5d-497b-b965-1d62a257c1c2");

            migrationBuilder.AddColumn<int>(
                name: "Seed",
                table: "TournamentPlayers",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7fe4cd1f-9ca5-4ebe-b16e-aa764d59e571", null, "Organizer", "ORGANIZER" },
                    { "a8edde04-986e-4845-b24b-538ee03c4a27", null, "Player", "PLAYER" },
                    { "c7550f56-dced-4fc2-98a0-874772e3ea31", null, "Referee", "REFEREE" },
                    { "f74a1997-0fb4-4cef-8150-91cc92c0dd8f", null, "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_AspNetUsers_PlayerAId",
                table: "Matches",
                column: "PlayerAId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_AspNetUsers_PlayerBId",
                table: "Matches",
                column: "PlayerBId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_AspNetUsers_RefereeId",
                table: "Matches",
                column: "RefereeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_AspNetUsers_WinnerId",
                table: "Matches",
                column: "WinnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Matches_NextMatchId",
                table: "Matches",
                column: "NextMatchId",
                principalTable: "Matches",
                principalColumn: "MatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Tournaments_TournamentId",
                table: "Matches",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "TournamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerProfiles_AspNetUsers_ApplicationUserId",
                table: "PlayerProfiles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayers_AspNetUsers_PlayerId",
                table: "TournamentPlayers",
                column: "PlayerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayers_Tournaments_TournamentId",
                table: "TournamentPlayers",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "TournamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_AspNetUsers_OrganizerId",
                table: "Tournaments",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_AspNetUsers_PlayerAId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_AspNetUsers_PlayerBId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_AspNetUsers_RefereeId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_AspNetUsers_WinnerId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Matches_NextMatchId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Tournaments_TournamentId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerProfiles_AspNetUsers_ApplicationUserId",
                table: "PlayerProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayers_AspNetUsers_PlayerId",
                table: "TournamentPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_TournamentPlayers_Tournaments_TournamentId",
                table: "TournamentPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tournaments_AspNetUsers_OrganizerId",
                table: "Tournaments");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7fe4cd1f-9ca5-4ebe-b16e-aa764d59e571");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a8edde04-986e-4845-b24b-538ee03c4a27");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c7550f56-dced-4fc2-98a0-874772e3ea31");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f74a1997-0fb4-4cef-8150-91cc92c0dd8f");

            migrationBuilder.DropColumn(
                name: "Seed",
                table: "TournamentPlayers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "597ac129-4fa0-4645-ad53-687245e84813", null, "Player", "PLAYER" },
                    { "73b5078d-0f91-4731-b673-6111845c55e6", null, "Admin", "ADMIN" },
                    { "e2fcb191-58d6-4823-98b1-e00fbbc36ba8", null, "Organizer", "ORGANIZER" },
                    { "f2905c1f-6a5d-497b-b965-1d62a257c1c2", null, "Referee", "REFEREE" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_AspNetUsers_PlayerAId",
                table: "Matches",
                column: "PlayerAId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_AspNetUsers_PlayerBId",
                table: "Matches",
                column: "PlayerBId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_AspNetUsers_RefereeId",
                table: "Matches",
                column: "RefereeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_AspNetUsers_WinnerId",
                table: "Matches",
                column: "WinnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Matches_NextMatchId",
                table: "Matches",
                column: "NextMatchId",
                principalTable: "Matches",
                principalColumn: "MatchId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Tournaments_TournamentId",
                table: "Matches",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "TournamentId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerProfiles_AspNetUsers_ApplicationUserId",
                table: "PlayerProfiles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayers_AspNetUsers_PlayerId",
                table: "TournamentPlayers",
                column: "PlayerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_TournamentPlayers_Tournaments_TournamentId",
                table: "TournamentPlayers",
                column: "TournamentId",
                principalTable: "Tournaments",
                principalColumn: "TournamentId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Tournaments_AspNetUsers_OrganizerId",
                table: "Tournaments",
                column: "OrganizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
