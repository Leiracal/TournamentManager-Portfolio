using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TournamentManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class RebuildRolesClean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4efc0160-ff19-4814-bb8e-11c501f4cf71");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "831627f2-2fdb-42a5-9b80-40b161e431d5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "deafdc72-e9b2-4771-8872-310205eedfb0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fad0db51-7431-4557-95ce-b48f0eb01e02");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4efc0160-ff19-4814-bb8e-11c501f4cf71", null, "Organizer", "ORGANIZER" },
                    { "831627f2-2fdb-42a5-9b80-40b161e431d5", null, "Admin", "ADMIN" },
                    { "deafdc72-e9b2-4771-8872-310205eedfb0", null, "Player", "PLAYER" },
                    { "fad0db51-7431-4557-95ce-b48f0eb01e02", null, "Referee", "REFEREE" }
                });
        }
    }
}
