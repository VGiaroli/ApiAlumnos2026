namespace ApiAlumnos2026.Models;

public class Asignatura
{
    public int AsignaturaId { get; set; }

    public string Descripcion { get; set; }

    public bool Eliminado { get; set; }

    public ICollection<NotaAlumno>? Notas { get; set; }
}
