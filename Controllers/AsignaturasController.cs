using System.IO.Compression;
using System.Security.Claims;
using ApiAlumnos2026.ClasesVistasVarias;
using ApiAlumnos2026.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAlumnos2026.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsignaturasController : ControllerBase
    {
        private readonly ApiAlumnos2026DbContext _context;

        public AsignaturasController(ApiAlumnos2026DbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<VistaAsignatura>>> GetAsignatura()
        {
            List<VistaAsignatura> vistaAsignaturas = new List<VistaAsignatura>();

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var usuario = _context.Users.Where(d => d.Id == userId).Single();

                //distinto al admin
                if (usuario.Email != "admin@gmail.com")
                {
                    //buscamos el docente relacionado
                    var docente = _context.Docentes.Where(d => d.Email == usuario.Email).SingleOrDefault();

                    if (docente != null)
                    {
                        var asignaturasDocente = _context.AsignaturaDocentes.Where(a => a.DocenteId == docente.DocenteId).ToList();

                        foreach (var asignaturaDocente in asignaturasDocente)
                        {
                            var asignatura = _context.Asignaturas
                            .Where(a => a.AsignaturaId == asignaturaDocente.AsignaturaId).Single();

                            var elemento = new VistaAsignatura
                            {
                                AsignaturaId = asignatura.AsignaturaId,
                                Descripcion = asignatura.Descripcion,
                                Eliminado = asignatura.Eliminado
                            };
                            vistaAsignaturas.Add(elemento);
                        }
                    }
                }
                else
                {
                    var asignaturas = await _context.Asignaturas
                    .Include(c => c.Carrera)
                    .OrderBy(c => c.Carrera.Nombre)
                    .ThenBy(a => a.Anio)
                    .ThenBy(n => n.Descripcion)
                    .ToListAsync();

                    foreach (var asignatura in asignaturas)
                    {
                        var elemento = new VistaAsignatura
                        {
                            AsignaturaId = asignatura.AsignaturaId,
                            Descripcion = asignatura.Descripcion,
                            CarreraID = asignatura.CarreraID,
                            Anio = asignatura.Anio,
                            Nombre = asignatura.Carrera.Nombre,
                            Eliminado = asignatura.Eliminado
                        };
                        vistaAsignaturas.Add(elemento);

                    }
                }
            }

            return vistaAsignaturas;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Asignatura>> GetObtenerAsignatura(int id)
        {
            var obtenerAlumno = await _context.Asignaturas.FindAsync(id);

            if (obtenerAlumno == null)
            {
                return NotFound();
            }

            return obtenerAlumno;
        }

        [HttpPost]
        // Asignatura es una clase
        // nuevaAsignatura es un parametro del metodo que contiene un objeto de tipo Asignatura
        // enviado desde el Front en formato Json.
        public async Task<IActionResult> PostAsignatura([FromBody] Asignatura nuevaAsignatura)
        {

            if (string.IsNullOrEmpty(nuevaAsignatura.Descripcion))
            {
                return BadRequest("Debe ingresar una descripción");
            }

            bool existeAsignatura = await _context.Asignaturas
                .AnyAsync(e => e.Descripcion.ToLower().Trim() == nuevaAsignatura.Descripcion.ToLower().Trim());
            if (existeAsignatura)
            {
                return BadRequest("Esta asignatura ya se encuentra registrada");
            }

            // Creamos un nuevo objeto de tipo Asignatura para guardar en la base de datos.
            // no usamos directamente el objeto que viene del Front para tener control
            // sobre qué datos queremos guardar.
            var guardarAsignatura = new Asignatura
            {
                Descripcion = nuevaAsignatura.Descripcion,
                CarreraID = nuevaAsignatura.CarreraID,
                Anio = nuevaAsignatura.Anio,
                Eliminado = false
            };

            _context.Asignaturas.Add(guardarAsignatura);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAsignatura", new { id = nuevaAsignatura.AsignaturaId }, nuevaAsignatura);

        }


        [HttpPut("{id}")]
        // Asignatura es una clase
        // nuevaAsignatura es un parametro del metodo que contiene un objeto de tipo Asignatura
        // enviado desde el Front en formato Json.

        public async Task<IActionResult> EditAsignatura(int Id, [FromBody] Asignatura asignaturas)
        {

            if (Id != asignaturas.AsignaturaId)
            {
                return BadRequest("El Id de la asignatura no coincide.");
            }

            if (string.IsNullOrEmpty(asignaturas.Descripcion))
            {
                return BadRequest("Debe ingresar una descripción");
            }


            // Creamos un nuevo objeto de tipo Asignatura para guardar en la base de datos.
            // no usamos directamente el objeto que viene del Front para tener control
            // sobre qué datos queremos editar y luego guardar.
            var asignaturaEditada = new Asignatura
            {
                AsignaturaId = asignaturas.AsignaturaId,
                Descripcion = asignaturas.Descripcion,
                CarreraID = asignaturas.CarreraID,
                Anio = asignaturas.Anio,
                Eliminado = asignaturas.Eliminado
            };

            _context.Entry(asignaturaEditada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AsignaturaExist(asignaturas.AsignaturaId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return NoContent();

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsignatura(int id)
        {

            var eliminarAsignatura = await _context.Asignaturas.FindAsync(id);
            if (eliminarAsignatura == null)
            {
                return NotFound();
            }

            eliminarAsignatura.Eliminado = true;
            _context.Entry(eliminarAsignatura).State = EntityState.Modified;

            try
            {
                _context.Asignaturas.Remove(eliminarAsignatura);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AsignaturaExist(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }


            return NoContent();

        }




        private bool AsignaturaExist(int id)
        {
            return _context.Asignaturas.Any(i => i.AsignaturaId == id);
        }
    }
}