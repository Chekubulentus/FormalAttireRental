using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalAttireBackend.Migrations
{
    /// <inheritdoc />
    public partial class uniqueclothecodenameindexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clothes_ClotheCode",
                table: "Clothes");

            migrationBuilder.DropIndex(
                name: "IX_Clothes_ClotheName",
                table: "Clothes");

            migrationBuilder.CreateIndex(
                name: "IX_Clothes_ClotheCode",
                table: "Clothes",
                column: "ClotheCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clothes_ClotheName",
                table: "Clothes",
                column: "ClotheName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Clothes_ClotheCode",
                table: "Clothes");

            migrationBuilder.DropIndex(
                name: "IX_Clothes_ClotheName",
                table: "Clothes");

            migrationBuilder.CreateIndex(
                name: "IX_Clothes_ClotheCode",
                table: "Clothes",
                column: "ClotheCode");

            migrationBuilder.CreateIndex(
                name: "IX_Clothes_ClotheName",
                table: "Clothes",
                column: "ClotheName");
        }
    }
}
