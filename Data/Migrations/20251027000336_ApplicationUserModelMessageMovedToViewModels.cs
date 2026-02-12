using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TournamentManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserModelMessageMovedToViewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerProfiles_ApplicationUserId",
                table: "PlayerProfiles");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "18fb5160-9f15-4a70-8c86-2ff53078e546");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "240d6c8d-2527-4b3e-8f6d-e8991313ff6e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ec6f3e97-04ec-4d71-a4ee-82ccfc6a5794");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f167a59a-0762-4c91-9387-d593c478e207");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "018c5eb7-8e64-4e5a-8410-8c9bec15970f", null, "Player", "PLAYER" },
                    { "27a215fd-aa8b-4f24-a219-1ae759bd902e", null, "Organizer", "ORGANIZER" },
                    { "7d2d3410-121d-474d-9652-888d0dde0258", null, "Admin", "ADMIN" },
                    { "f4bfcf99-e3ea-479b-9d8f-7e4504b3049f", null, "Referee", "REFEREE" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProfiles_ApplicationUserId",
                table: "PlayerProfiles",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlayerProfiles_ApplicationUserId",
                table: "PlayerProfiles");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "018c5eb7-8e64-4e5a-8410-8c9bec15970f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "27a215fd-aa8b-4f24-a219-1ae759bd902e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7d2d3410-121d-474d-9652-888d0dde0258");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f4bfcf99-e3ea-479b-9d8f-7e4504b3049f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "18fb5160-9f15-4a70-8c86-2ff53078e546", null, "Player", "PLAYER" },
                    { "240d6c8d-2527-4b3e-8f6d-e8991313ff6e", null, "Referee", "REFEREE" },
                    { "ec6f3e97-04ec-4d71-a4ee-82ccfc6a5794", null, "Organizer", "ORGANIZER" },
                    { "f167a59a-0762-4c91-9387-d593c478e207", null, "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerProfiles_ApplicationUserId",
                table: "PlayerProfiles",
                column: "ApplicationUserId");
        }
    }
}
