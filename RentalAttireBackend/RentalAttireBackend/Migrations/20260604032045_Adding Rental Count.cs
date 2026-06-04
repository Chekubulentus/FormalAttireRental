using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalAttireBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddingRentalCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RentalCount",
                table: "Clothes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RentalCount",
                table: "Clothes");
        }
    }
}
