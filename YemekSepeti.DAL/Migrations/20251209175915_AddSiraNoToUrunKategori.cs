using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekSepeti.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSiraNoToUrunKategori : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SiraNo",
                table: "UrunKategoriler",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiraNo",
                table: "UrunKategoriler");
        }
    }
}
