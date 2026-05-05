namespace ApiAlumnos2026.Models;

public class Alumno
{
    public int AlumnoId { get; set; }

    public string NombreCompleto { get; set; }

    public int DNI { get; set; }

    public string Domicilio { get; set; }

    public Sexo Sexo { get; set; }

    public ICollection<NotaAlumno>? Notas { get; set; }
}
