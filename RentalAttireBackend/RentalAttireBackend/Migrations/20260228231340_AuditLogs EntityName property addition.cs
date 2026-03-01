using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalAttireBackend.Migrations
{
    /// <inheritdoc />
    public partial class AuditLogsEntityNamepropertyaddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "AuditLogs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "AuditLogs");
        }
    }
}
