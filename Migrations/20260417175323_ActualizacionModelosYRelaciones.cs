using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiAlumnos2026.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionModelosYRelaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreAlumno",
                table: "NotaAlumnos");

            migrationBuilder.DropColumn(
                name: "NumeroDNI",
                table: "NotaAlumnos");

            migrationBuilder.RenameColumn(
                name: "Domiciliio",
                table: "Alumnos",
                newName: "Domicilio");

            migrationBuilder.AddColumn<int>(
                name: "Sexo",
                table: "Alumnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_NotaAlumnos_AlumnoId",
                table: "NotaAlumnos",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaAlumnos_AsignaturaId",
                table: "NotaAlumnos",
                column: "AsignaturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotaAlumnos_Alumnos_AlumnoId",
                table: "NotaAlumnos",
                column: "AlumnoId",
                principalTable: "Alumnos",
                principalColumn: "AlumnoId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotaAlumnos_Asignaturas_AsignaturaId",
                table: "NotaAlumnos",
                column: "AsignaturaId",
                principalTable: "Asignaturas",
                principalColumn: "AsignaturaId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotaAlumnos_Alumnos_AlumnoId",
                table: "NotaAlumnos");

            migrationBuilder.DropForeignKey(
                name: "FK_NotaAlumnos_Asignaturas_AsignaturaId",
                table: "NotaAlumnos");

            migrationBuilder.DropIndex(
                name: "IX_NotaAlumnos_AlumnoId",
                table: "NotaAlumnos");

            migrationBuilder.DropIndex(
                name: "IX_NotaAlumnos_AsignaturaId",
                table: "NotaAlumnos");

            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "Alumnos");

            migrationBuilder.RenameColumn(
                name: "Domicilio",
                table: "Alumnos",
                newName: "Domiciliio");

            migrationBuilder.AddColumn<string>(
                name: "NombreAlumno",
                table: "NotaAlumnos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumeroDNI",
                table: "NotaAlumnos",
                type: "int",
                nullable: true);
        }
    }
}
