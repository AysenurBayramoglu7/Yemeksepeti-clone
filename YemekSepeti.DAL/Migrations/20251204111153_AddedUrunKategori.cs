using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekSepeti.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedUrunKategori : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UrunKategoriID",
                table: "Urunler",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UrunKategoriler",
                columns: table => new
                {
                    UrunKategoriID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunKategoriAd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrunKategoriler", x => x.UrunKategoriID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_UrunKategoriID",
                table: "Urunler",
                column: "UrunKategoriID");

            migrationBuilder.AddForeignKey(
                name: "FK_Urunler_UrunKategoriler_UrunKategoriID",
                table: "Urunler",
                column: "UrunKategoriID",
                principalTable: "UrunKategoriler",
                principalColumn: "UrunKategoriID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Urunler_UrunKategoriler_UrunKategoriID",
                table: "Urunler");

            migrationBuilder.DropTable(
                name: "UrunKategoriler");

            migrationBuilder.DropIndex(
                name: "IX_Urunler_UrunKategoriID",
                table: "Urunler");

            migrationBuilder.DropColumn(
                name: "UrunKategoriID",
                table: "Urunler");
        }
    }
}
