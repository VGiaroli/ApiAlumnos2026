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
        [HttpPost("promedioasignaturas")]
        public async Task<ActionResult<IEnumerable<VistaPromedioAsignatura>>> PostAlumno(FiltroNotaAlumno filtro)
        {
            List<VistaPromedioAsignatura> asignaturasMostrar = new List<VistaPromedioAsignatura>();

            var asignaturas = await _context.Asignaturas.ToListAsync();

            if (filtro.AsignaturaId > 0)
            {
                asignaturas = asignaturas.Where(a => a.AsignaturaId == filtro.AsignaturaId).ToList();
            }

            foreach (var asignatura in asignaturas)
            {
                var notasAsignatura = await _context.NotaAlumnos.Where(a => a.AsignaturaId == asignatura.AsignaturaId).ToListAsync();

                if (filtro.AlumnoId > 0)
                {
                    notasAsignatura = notasAsignatura.Where(a => a.AlumnoId == filtro.AlumnoId).ToList();
                }

                DateTime fechaDesde = new DateTime();
                bool fechaDesdeValida = DateTime.TryParse(filtro.FechaDesde, out fechaDesde);

                DateTime fechaHasta = new DateTime();
                bool fechaHastaValida = DateTime.TryParse(filtro.FechaHasta, out fechaHasta);

                if (fechaDesdeValida && fechaHastaValida)
                {
                    fechaHasta = fechaHasta.AddHours(23);
                    fechaHasta = fechaHasta.AddMinutes(59);
                    fechaHasta = fechaHasta.AddSeconds(59);
                    notasAsignatura = notasAsignatura.Where(t => t.Fecha >= fechaDesde && t.Fecha <= fechaHasta).ToList();
                }


                if (notasAsignatura.Count > 0)
                {
                    var alumnoMostrar = new VistaPromedioAsignatura
                    {
                        AsignaturaId = asignatura.AsignaturaId,
                        Descripcion = asignatura.Descripcion,
                        Promedio = decimal.Round(Convert.ToDecimal(notasAsignatura.Sum(n => n.Nota)) / notasAsignatura.Count(), 2)
                    };
                    asignaturasMostrar.Add(alumnoMostrar);
                }
            }

            asignaturasMostrar = asignaturasMostrar.OrderBy(a => a.Descripcion).ToList();

            return asignaturasMostrar.ToList();
        }


        [HttpGet("HistorialNotas/{id}")]
        public async Task<IActionResult> HistorialNotas(int id)
        {
            var historial = await _context.HistorialNotaAlumnos
                .Where(h => h.NotaAlumnoID == id)
                .OrderByDescending(h => h.FechaCambio)
                .Select(h => new VistaHistorialNotaAlumno
                {
                    HistorialNotaAlumnoID = h.HistorialNotaAlumnoID,
                    NotaAlumnoID = h.NotaAlumnoID,
                    FechaCambioString = h.FechaCambio.ToString("dd/MM/yyyy HH:mm"),
                    CampoModificado = h.CampoModificado,
                    ValorAnterior = h.ValorAnterior,
                    ValorNuevo = h.ValorNuevo
                })
                .ToListAsync();

            return Ok(historial);
        }


        [HttpGet("HistorialAlumnos/{id}")]
        public async Task<IActionResult> HistorialAlumnos(int id)
        {
            var historialAlumno = await _context.HistorialAlumnos
                .Where(h => h.AlumnoId == id)
                .OrderByDescending(h => h.FechaCambio)
                .Select(h => new VistaHistorialAlumno
                {
                    HistorialAlumnoId = h.HistorialAlumnoID,
                    AlumnoId = h.AlumnoId,
                    FechaCambioString = h.FechaCambio.ToString("dd/MM/yyyy HH:mm"),
                    CampoModificado = h.CampoModificado,
                    ValorAnterior = h.ValorAnterior,
                    ValorNuevo = h.ValorNuevo
                })
                .ToListAsync();

            return Ok(historialAlumno);
        }

        [HttpGet("HistorialDocentes/{id}")]
        public async Task<IActionResult> HistorialDocentes(int id)
        {
            var historialDocente = await _context.HistorialDocentes
                .Where(d => d.DocenteId == id)
                .OrderByDescending(d => d.FechaCambio)
                .Select(d => new VistaHistorialDocente
                {
                    HistorialDocenteId = d.HistorialDocenteID,
                    DocenteId = d.DocenteId,
                    FechaCambioString = d.FechaCambio.ToString("dd/MM/yyyy HH:mm"),
                    CampoModificado = d.CampoModificado,
                    ValorAnterior = d.ValorAnterior,
                    ValorNuevo = d.ValorNuevo
                })
                .ToListAsync();

            return Ok(historialDocente);
        }

    }
}

