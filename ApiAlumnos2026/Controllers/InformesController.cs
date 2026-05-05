using System.Threading.Tasks;
using ApiAlumnos2026.ClasesVistasVarias;
using ApiAlumnos2026.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;


namespace ApiAlumnos2026.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class InformesController : ControllerBase
    {
        private readonly ApiAlumnos2026DbContext _context;

        public InformesController(ApiAlumnos2026DbContext context)
        {
            _context = context;
        }
        
        [HttpPost("promedioAlumno")]
        public async Task<ActionResult<IEnumerable<VistaPromedioAlumno>>> PostAsignatura(FiltroNotaAlumno filtro)
        {

            List<VistaPromedioAlumno> alumnosMostrar = new List<VistaPromedioAlumno>();
            
            var alumnos = await _context.Alumnos.ToListAsync();
            foreach (var alumno in alumnos)
            {
                var notasAlumno = await _context.NotaAlumnos.Where(a => a.AlumnoId == alumno.AlumnoId).ToListAsync();
                if(filtro.AsignaturaId > 0)
                {
                    notasAlumno = notasAlumno.Where(a => a.AsignaturaId == filtro.AsignaturaId).ToList();
                }

                DateTime fechaDesde = new DateTime();
                bool fechaDesdeValida = DateTime.TryParse(filtro.FechaDesde, out fechaDesde);

                DateTime fechaHasta = new DateTime();
                bool fechaHastaValida = DateTime.TryParse(filtro.FechaHasta, out fechaHasta);

                if(fechaDesdeValida && fechaHastaValida)
                {
                    fechaHasta = fechaHasta.AddHours(23);
                    fechaHasta = fechaHasta.AddMinutes(59);
                    fechaHasta = fechaHasta.AddMinutes(59);
                    notasAlumno = notasAlumno.Where(t => t.Fecha >= fechaDesde && t.Fecha <= fechaHasta).ToList();

                }

                if(notasAlumno.Count > 0)
                {
                    var alumnoMostrar = new VistaPromedioAlumno
                    {
                      NombreCompleto = alumno.NombreCompleto,
                      DNI = alumno.DNI,
                      Promedio = decimal.Round(Convert.ToDecimal(notasAlumno.Sum(n => n.Nota)) / notasAlumno.Count(), 2)  
                    };
                    alumnosMostrar.Add(alumnoMostrar);
                }
                
            }

            alumnosMostrar = alumnosMostrar.OrderBy(a => a.NombreCompleto).ToList();

            return alumnosMostrar.ToList();
        }
    }
}