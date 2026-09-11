using System.ComponentModel.DataAnnotations;

namespace ApiAlumnos2026.Models
{
    public class HistorialAlumno
    {
        [Key]
        public int HistorialAlumnoID {get; set;}

        public int AlumnoId {get; set;}

        public string? CampoModificado {get; set;}

        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }

        public DateTime FechaCambio {get; set;}
    }

    public class VistaHistorialAlumno
    {
        public int HistorialAlumnoId { get; set; }
        public int AlumnoId { get; set; }
        public string? FechaCambioString { get; set; }
        public string? CampoModificado { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
    }
}