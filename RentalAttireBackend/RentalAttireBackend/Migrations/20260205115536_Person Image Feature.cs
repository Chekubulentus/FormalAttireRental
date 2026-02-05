using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalAttireBackend.Migrations
{
    /// <inheritdoc />
    public partial class PersonImageFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                table: "People",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                table: "People");
        }
    }
}
