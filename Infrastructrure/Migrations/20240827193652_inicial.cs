using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructrure.Migrations
{
    /// <inheritdoc />
    public partial class inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artista",
                columns: table => new
                {
                    ArtistaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artista", x => x.ArtistaId);
                });

            migrationBuilder.CreateTable(
                name: "Genero",
                columns: table => new
                {
                    GeneroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genero", x => x.GeneroId);
                });

            migrationBuilder.CreateTable(
                name: "Album",
                columns: table => new
                {
                    AlbumId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArtistaId = table.Column<int>(type: "int", nullable: false),
                    GeneroId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Album", x => x.AlbumId);
                    table.ForeignKey(
                        name: "FK_Album_Artista_ArtistaId",
                        column: x => x.ArtistaId,
                        principalTable: "Artista",
                        principalColumn: "ArtistaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Album_Genero_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Genero",
                        principalColumn: "GeneroId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Artista",
                columns: new[] { "ArtistaId", "Nombre" },
                values: new object[,]
                {
                    { 1, "Michael Jackson" },
                    { 2, "Bruno Mars" },
                    { 3, "Si lees esto vas a promocionar Proyecto Software" },
                    { 4, "Se me acabaron las ganas de buscar mas artistas XD" }
                });

            migrationBuilder.InsertData(
                table: "Genero",
                columns: new[] { "GeneroId", "Nombre" },
                values: new object[,]
                {
                    { 1, "Pop" },
                    { 2, "Rock" },
                    { 3, "Electronica" },
                    { 4, "Jazz" }
                });

            migrationBuilder.InsertData(
                table: "Album",
                columns: new[] { "AlbumId", "ArtistaId", "GeneroId", "Nombre" },
                values: new object[,]
                {
                    { 1, 1, 1, "Thriller" },
                    { 2, 2, 2, "24k Magic" },
                    { 3, 3, 3, "Usa tu imaginacion para este" },
                    { 4, 4, 4, "Tambien se me fueron las ganas de buscar albumes" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Album_ArtistaId",
                table: "Album",
                column: "ArtistaId");

            migrationBuilder.CreateIndex(
                name: "IX_Album_GeneroId",
                table: "Album",
                column: "GeneroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Album");

            migrationBuilder.DropTable(
                name: "Artista");

            migrationBuilder.DropTable(
                name: "Genero");
        }
    }
}
