using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RentalAttireBackend.Migrations
{
    /// <inheritdoc />
    public partial class categoryinitialdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "ArchivedAt", "ArchivedBy", "CategoryCode", "CategoryName", "CreatedAt", "CreatedBy", "Description", "EntityType", "IsActive", "IsDeleted", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, null, "", "C-0001", "Gown", new DateTime(2025, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Papat", "Used for formal parties.", "Category", true, false, null, "" },
                    { 2, null, "", "C-0002", "Tuxedo", new DateTime(2025, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Papat", "Used for formal parties.", "Category", true, false, null, "" },
                    { 3, null, "", "C-0003", "Barong", new DateTime(2025, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Papat", "Used for  being dead.", "Category", true, false, null, "" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
