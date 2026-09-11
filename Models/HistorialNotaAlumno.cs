using System.ComponentModel.DataAnnotations;

namespace ApiAlumnos2026.Models
{
    public class HistorialNotaAlumno
    {
        [Key]
        public int HistorialNotaAlumnoID { get; set; }
        public int NotaAlumnoID { get; set; }
        public DateTime FechaCambio { get; set; }
        public string? CampoModificado { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
    }

    public class VistaHistorialNotaAlumno
    {
        public int HistorialNotaAlumnoID { get; set; }
        public int NotaAlumnoID { get; set; }
        public string? FechaCambioString { get; set; }
        public string? CampoModificado { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
    }
}