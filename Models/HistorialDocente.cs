namespace ApiAlumnos2026.Models
{
    public class HistorialDocente
    {
        public int HistorialDocenteID { get; set; }

        public int DocenteId { get; set; }

        public string? CampoModificado { get; set; }

        public string? ValorAnterior { get; set; }

        public string? ValorNuevo { get; set; }

        public DateTime FechaCambio { get; set; }
    }

    public class VistaHistorialDocente
    {
        public int HistorialDocenteId { get; set; }

        public int DocenteId { get; set; }

        public string? CampoModificado { get; set; }

        public string? ValorAnterior { get; set; }

        public string? ValorNuevo { get; set; }

        public string? FechaCambioString { get; set; }

    }
}