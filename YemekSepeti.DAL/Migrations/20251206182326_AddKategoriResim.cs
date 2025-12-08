using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekSepeti.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddKategoriResim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KategoriResimUrl",
                table: "Kategoriler",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KategoriResimUrl",
                table: "Kategoriler");
        }
    }
}
