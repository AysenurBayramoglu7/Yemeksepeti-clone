using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekSepeti.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRestoranResimUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RestoranResimUrl",
                table: "Restoran",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestoranResimUrl",
                table: "Restoran");
        }
    }
}
