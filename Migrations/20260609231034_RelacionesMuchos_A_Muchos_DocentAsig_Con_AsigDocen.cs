using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiAlumnos2026.Migrations
{
    /// <inheritdoc />
    public partial class RelacionesMuchos_A_Muchos_DocentAsig_Con_AsigDocen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AsignaturaAsignaturaDocente",
                columns: table => new
                {
                    AsignaturaDocentesAsignaturaDocenteId = table.Column<int>(type: "int", nullable: false),
                    AsignaturasAsignaturaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignaturaAsignaturaDocente", x => new { x.AsignaturaDocentesAsignaturaDocenteId, x.AsignaturasAsignaturaId });
                    table.ForeignKey(
                        name: "FK_AsignaturaAsignaturaDocente_AsignaturaDocentes_AsignaturaDocentesAsignaturaDocenteId",
                        column: x => x.AsignaturaDocentesAsignaturaDocenteId,
                        principalTable: "AsignaturaDocentes",
                        principalColumn: "AsignaturaDocenteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsignaturaAsignaturaDocente_Asignaturas_AsignaturasAsignaturaId",
                        column: x => x.AsignaturasAsignaturaId,
                        principalTable: "Asignaturas",
                        principalColumn: "AsignaturaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AsignaturaDocenteDocente",
                columns: table => new
                {
                    AsignaturaDocentesAsignaturaDocenteId = table.Column<int>(type: "int", nullable: false),
                    DocentesDocenteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignaturaDocenteDocente", x => new { x.AsignaturaDocentesAsignaturaDocenteId, x.DocentesDocenteId });
                    table.ForeignKey(
                        name: "FK_AsignaturaDocenteDocente_AsignaturaDocentes_AsignaturaDocentesAsignaturaDocenteId",
                        column: x => x.AsignaturaDocentesAsignaturaDocenteId,
                        principalTable: "AsignaturaDocentes",
                        principalColumn: "AsignaturaDocenteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsignaturaDocenteDocente_Docentes_DocentesDocenteId",
                        column: x => x.DocentesDocenteId,
                        principalTable: "Docentes",
                        principalColumn: "DocenteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturaAsignaturaDocente_AsignaturasAsignaturaId",
                table: "AsignaturaAsignaturaDocente",
                column: "AsignaturasAsignaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignaturaDocenteDocente_DocentesDocenteId",
                table: "AsignaturaDocenteDocente",
                column: "DocentesDocenteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignaturaAsignaturaDocente");

            migrationBuilder.DropTable(
                name: "AsignaturaDocenteDocente");
        }
    }
}
