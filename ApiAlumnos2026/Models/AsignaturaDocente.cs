namespace ApiAlumnos2026.Models
{
    public class AsignaturaDocente
    {
        public int AsignaturaDocenteId { get; set; }

        public int DocenteId { get; set; }
        //cada fila se asocia a un solo docente
        public Docente? Docente {get; set;}

        public int AsignaturaId {get; set;}
        //cada fila se asocia a una sola asignatura
        public Asignatura? Asignatura { get; set;}

    }
}