namespace ApiAlumnos2026.Models;

public class NotaAlumno
{
    public int NotaAlumnoId { get; set; }

    public int Nota { get; set; }

    public int AlumnoId { get; set; }

    //agregar asignaturaID
    public int AsignaturaId { get; set; }

    public virtual Alumno? Alumno { get; set; }

    public virtual Asignatura? Asignatura { get; set; }

    public DateTime Fecha {get; set;}
}