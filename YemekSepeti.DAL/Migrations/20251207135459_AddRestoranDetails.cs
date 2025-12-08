using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekSepeti.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRestoranDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinSiparisTutar",
                table: "Restoran",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OrtalamaSure",
                table: "Restoran",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinSiparisTutar",
                table: "Restoran");

            migrationBuilder.DropColumn(
                name: "OrtalamaSure",
                table: "Restoran");
        }
    }
}
