using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalAttireBackend.Migrations
{
    /// <inheritdoc />
    public partial class addingmainadmininitialdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "ArchivedAt", "ArchivedBy", "CreatedAt", "CreatedBy", "Department", "EmployeeCode", "EntityType", "IsActive", "IsDeleted", "RestoredAt", "RestoredBy", "RoleId", "Salary", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { 1, null, "", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Administration", "EMP-000001", "Employee", true, false, null, "", 1, 0.0, null, "", 1 });

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "Id", "Age", "ArchivedAt", "ArchivedBy", "Barangay", "City", "CreatedAt", "CreatedBy", "EntityType", "FirstName", "Gender", "IsActive", "IsDeleted", "LastName", "MaritalStatus", "MiddleName", "PhoneNumber", "PostalCode", "ProfileImagePath", "Province", "RestoredAt", "RestoredBy", "Street", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 2, 30, null, "", "System Barangay", "System City", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Person", "Main", 2, true, false, "Administrator", 0, "System", "09000000000", "0000", null, "System Province", null, "", "System Street", null, "" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ArchivedAt", "ArchivedBy", "CreatedAt", "CreatedBy", "Email", "EntityType", "HashedPassword", "IsActive", "IsDeleted", "IsGoogleAccount", "PersonId", "RefreshToken", "RefreshTokenExpiryTime", "RestoredAt", "RestoredBy", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 2, null, "", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "System", "admin", "User", "admin", true, false, false, 2, "", null, null, "", null, "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "People",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
