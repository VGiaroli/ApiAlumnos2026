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
                if (filtro.AsignaturaId > 0)
                {
                    notasAlumno = notasAlumno.Where(a => a.AsignaturaId == filtro.AsignaturaId).ToList();
                }

                DateTime fechaDesde = new DateTime();
                bool fechaDesdeValida = DateTime.TryParse(filtro.FechaDesde, out fechaDesde);

                DateTime fechaHasta = new DateTime();
                bool fechaHastaValida = DateTime.TryParse(filtro.FechaHasta, out fechaHasta);

                if (fechaDesdeValida && fechaHastaValida)
                {
                    fechaHasta = fechaHasta.AddHours(23);
                    fechaHasta = fechaHasta.AddMinutes(59);
                    fechaHasta = fechaHasta.AddMinutes(59);
                    notasAlumno = notasAlumno.Where(t => t.Fecha >= fechaDesde && t.Fecha <= fechaHasta).ToList();

                }

                if (notasAlumno.Count > 0)
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

            //VERIFICAR DESPUES CON ESTO
            // alumnosMostrar = alumnosMostrar.OrderByDescending(n => n.Promedio).ToList();
            alumnosMostrar = alumnosMostrar.OrderBy(a => a.NombreCompleto).ToList();

            return alumnosMostrar.ToList();
        }

        //Desarrollar un informe que muestre por Asignatura el promedio de notas registradas
        //Permitir filtrar fechas desde y hasta, por asignatura y por alumno
        [HttpPost("promedioRegistro")]
        public async Task<ActionResult<IEnumerable<VistaPromedioAlumno>>> PostAlumno (FiltroNotaAlumno filtro)
        {
            List<VistaPromedioAlumno> asignaturasMostrar = new List<VistaPromedioAlumno>();

            var asignaturas = await _context.Alumnos.ToListAsync();
            foreach (var asignatura in asignaturas)
            {
                var promediosRegistro = await _context.NotaAlumnos.Where(a => a.AlumnoId == asignatura.AlumnoId).ToListAsync();
                if (filtro.AsignaturaId > 0)
                {
                    promediosRegistro = promediosRegistro.Where(a => a.AsignaturaId == filtro.AsignaturaId).ToList();
                }

                DateTime fechaDesde = new DateTime();
                bool fechaDesdeValida = DateTime.TryParse(filtro.FechaDesde, out fechaDesde);

                DateTime fechaHasta = new DateTime();
                bool fechaHastaValida = DateTime.TryParse(filtro.FechaHasta, out fechaHasta);

                if (fechaDesdeValida && fechaHastaValida)
                {
                    fechaHasta = fechaHasta.AddHours(23);
                    fechaHasta = fechaHasta.AddMinutes(59);
                    fechaHasta = fechaHasta.AddMinutes(59);
                    promediosRegistro = promediosRegistro.Where(t => t.Fecha >= fechaDesde && t.Fecha <= fechaHasta).ToList();

                }

                if (promediosRegistro.Count > 0)
                {
                    var promedioMostrar = new VistaPromedioAlumno
                    {
                        NombreCompleto = asignatura.NombreCompleto,
                        Promedio = decimal.Round(Convert.ToDecimal(promediosRegistro.Sum(n => n.Nota)) / promediosRegistro.Count(), 2)
                    };
                    asignaturasMostrar.Add(promedioMostrar);
                }


            }

            //VERIFICAR DESPUES CON ESTO
            // alumnosMostrar = alumnosMostrar.OrderByDescending(n => n.Promedio).ToList();
            asignaturasMostrar = asignaturasMostrar.OrderBy(a => a.NombreCompleto).ToList();
            

            return asignaturasMostrar.ToList();

        }

    }
}