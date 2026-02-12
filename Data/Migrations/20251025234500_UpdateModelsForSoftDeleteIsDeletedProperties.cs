using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TournamentManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsForSoftDeleteIsDeletedProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<bool>(
                name: "IsTournamentDeleted",
                table: "Tournaments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPlayerProfileDeleted",
                table: "PlayerProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMatchDeleted",
                table: "Matches",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "IsTournamentDeleted",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "IsPlayerProfileDeleted",
                table: "PlayerProfiles");

            migrationBuilder.DropColumn(
                name: "IsMatchDeleted",
                table: "Matches");

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
        }
    }
}
