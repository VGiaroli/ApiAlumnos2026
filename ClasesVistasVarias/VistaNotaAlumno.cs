namespace ApiAlumnos2026.ClasesVistasVarias
{
    public class VistaNotaAlumno
    {
        public int NotaAlumnoId { get; set; }
        public int AlumnoId { get; set; }
        public string? NombreCompleto { get; set; }
        public int AsignaturaId { get; set; }

        public string? AsignaturaNombre { get; set; }
        public string? FechaString {get;set;}
         public string? FechaStringInput {get;set;}
        public int Nota { get; set; }
        public int DNI { get; set; }
    }
}