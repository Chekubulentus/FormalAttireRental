using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalAttireBackend.Migrations
{
    /// <inheritdoc />
    public partial class fixmainadminuserIdseeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Employees"" SET ""UserId"" = 2
                WHERE ""Id"" = 1
                AND NOT EXISTS (
                    SELECT 1 FROM ""Employees"" WHERE ""UserId"" = 2
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: 1);
        }
    }
}