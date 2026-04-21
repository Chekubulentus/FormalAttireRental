using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalAttireBackend.Migrations
{
    /// <inheritdoc />
    public partial class addingrestorationauditproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RestoredAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RestoredAt",
                table: "Roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "Roles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RestoredAt",
                table: "Rentals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "Rentals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RestoredAt",
                table: "People",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "People",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RestoredAt",
                table: "Employees",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RestoredAt",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RestoredAt",
                table: "Clothes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "Clothes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RestoredAt",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestoredBy",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "RestoredAt", "RestoredBy" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "RestoredAt", "RestoredBy" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "RestoredAt", "RestoredBy" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "RestoredAt", "RestoredBy" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "RestoredAt", "RestoredBy" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "RestoredAt", "RestoredBy" },
                values: new object[] { null, "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestoredAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RestoredAt",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RestoredAt",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "RestoredAt",
                table: "People");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "People");

            migrationBuilder.DropColumn(
                name: "RestoredAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RestoredAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RestoredAt",
                table: "Clothes");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "Clothes");

            migrationBuilder.DropColumn(
                name: "RestoredAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "RestoredBy",
                table: "Categories");
        }
    }
}
