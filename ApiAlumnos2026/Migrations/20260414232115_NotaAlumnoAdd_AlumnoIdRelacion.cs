using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiAlumnos2026.Migrations
{
    /// <inheritdoc />
    public partial class NotaAlumnoAdd_AlumnoIdRelacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlumnoId",
                table: "NotaAlumnos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlumnoId",
                table: "NotaAlumnos");
        }
    }
}
