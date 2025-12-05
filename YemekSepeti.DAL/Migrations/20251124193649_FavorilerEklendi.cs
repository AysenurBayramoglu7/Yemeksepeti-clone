using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YemekSepeti.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FavorilerEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavoriRestoranlar",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false),
                    RestoranID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriRestoranlar", x => new { x.KullaniciID, x.RestoranID });
                    table.ForeignKey(
                        name: "FK_FavoriRestoranlar_Kullanicilar_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FavoriRestoranlar_Restoran_RestoranID",
                        column: x => x.RestoranID,
                        principalTable: "Restoran",
                        principalColumn: "RestoranID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriRestoranlar_RestoranID",
                table: "FavoriRestoranlar",
                column: "RestoranID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriRestoranlar");
        }
    }
}
