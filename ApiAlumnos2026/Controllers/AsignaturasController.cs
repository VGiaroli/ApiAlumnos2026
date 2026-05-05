using ApiAlumnos2026.Models;
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
        public async Task<ActionResult<IEnumerable<Asignatura>>> GetAsignatura()
        {
            return await _context.Asignaturas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Asignatura>> GetObtenerAsignatura(int id)
        {
            var obtenerAlumno = await _context.Asignaturas.FindAsync(id);
            
            if(obtenerAlumno == null)
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
            if(existeAsignatura)
            {
                return BadRequest("Esta asignatura ya se encuentra registrada");
            }

            // Creamos un nuevo objeto de tipo Asignatura para guardar en la base de datos.
            // no usamos directamente el objeto que viene del Front para tener control
            // sobre qué datos queremos guardar.
            var guardarAsignatura = new Asignatura
            {
                Descripcion = nuevaAsignatura.Descripcion,
                Eliminado = false
            };
            
            _context.Asignaturas.Add(guardarAsignatura);
            await _context.SaveChangesAsync();

            return Ok(guardarAsignatura);
                
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
                Eliminado = asignaturas.Eliminado
            };

            _context.Entry(asignaturaEditada).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!AsignaturaExist(asignaturas.AsignaturaId))
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

            var eliminarAlumno = await _context.Asignaturas.FindAsync(id);
            if(eliminarAlumno == null)
            {
                return NotFound(); 
            }
            
            try
            {
                _context.Asignaturas.Remove(eliminarAlumno);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!AsignaturaExist(id))
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