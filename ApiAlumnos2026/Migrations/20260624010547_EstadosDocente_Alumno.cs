using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiAlumnos2026.Migrations
{
    /// <inheritdoc />
    public partial class EstadosDocente_Alumno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Docentes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Alumnos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Alumnos");
        }
    }
}
