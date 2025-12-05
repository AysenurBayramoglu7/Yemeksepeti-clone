using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YemekSepeti.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RollerInitialSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roller",
                columns: new[] { "RolID", "RolAd" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "RestoranSahibi" },
                    { 3, "Musteri" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roller",
                keyColumn: "RolID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roller",
                keyColumn: "RolID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roller",
                keyColumn: "RolID",
                keyValue: 3);
        }
    }
}
