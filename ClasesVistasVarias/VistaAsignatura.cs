namespace ApiAlumnos2026.ClasesVistasVarias
{
    public class VistaAsignatura
    {
        public int AsignaturaId { get; set; }
        public string? Descripcion { get; set; }
        public bool Eliminado { get; set; }

        public int CarreraID {get; set;}

        public int Anio {get; set;}

        public string? Nombre {get; set;}
    }
}