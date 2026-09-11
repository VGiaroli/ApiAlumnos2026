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
    public class CarrerasController : ControllerBase
    {
        private readonly ApiAlumnos2026DbContext _context;

        public CarrerasController(ApiAlumnos2026DbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Carrera>>> GetCarrera()
        {
            return await _context.Carreras
            .OrderBy(n => n.Nombre)
            .ToListAsync(); 
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Carrera>> GetObtenerCarrera(int id)
        {
            var obtenerCarrera = await _context.Carreras.FindAsync(id);

            if (obtenerCarrera == null)
            {
                return NotFound();
            }

            return obtenerCarrera;
        }

        [HttpPost]
        // Asignatura es una clase
        // nuevaAsignatura es un parametro del metodo que contiene un objeto de tipo Asignatura
        // enviado desde el Front en formato Json.
        public async Task<IActionResult> PostCarrera([FromBody] Carrera nuevaCarrera)
        {

            if (string.IsNullOrEmpty(nuevaCarrera.Nombre))
            {
                return BadRequest("Debe ingresar un nombre");
            }

            bool existeCarrera = await _context.Carreras
                .AnyAsync(e => e.Nombre.ToLower().Trim() == nuevaCarrera.Nombre.ToLower().Trim());
            if (existeCarrera)
            {
                return BadRequest("Esta carrera ya se encuentra registrada");
            }

            // Creamos un nuevo objeto de tipo Asignatura para guardar en la base de datos.
            // no usamos directamente el objeto que viene del Front para tener control
            // sobre qué datos queremos guardar.
            var guardarCarrera = new Carrera
            {
                Nombre = nuevaCarrera.Nombre,
                Duracion = nuevaCarrera.Duracion,
                Eliminado = false
            };

            _context.Carreras.Add(guardarCarrera);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarrera", new { id = nuevaCarrera.CarreraID }, nuevaCarrera);

        }


        [HttpPut("{id}")]
        // Asignatura es una clase
        // nuevaAsignatura es un parametro del metodo que contiene un objeto de tipo Asignatura
        // enviado desde el Front en formato Json.

        public async Task<IActionResult> EditCarrera(int Id, [FromBody] Carrera carrera)
        {

            if (Id != carrera.CarreraID)
            {
                return BadRequest("El Id de la carrera no coincide.");
            }

            if (string.IsNullOrEmpty(carrera.Nombre))
            {
                return BadRequest("Debe ingresar un nombre");
            }


            // Creamos un nuevo objeto de tipo Asignatura para guardar en la base de datos.
            // no usamos directamente el objeto que viene del Front para tener control
            // sobre qué datos queremos editar y luego guardar.
            var carreraEditada = new Carrera
            {
                CarreraID = carrera.CarreraID,
                Nombre = carrera.Nombre,
                Duracion = carrera.Duracion,
                Eliminado = carrera.Eliminado
            };

            _context.Entry(carreraEditada).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarreraExist(carrera.CarreraID))
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
        public async Task<IActionResult> DeleteCarrera(int id)
        {

            var eliminarCarrera = await _context.Carreras.FindAsync(id);
            if (eliminarCarrera == null)
            {
                return NotFound();
            }

            eliminarCarrera.Eliminado = true;
            _context.Entry(eliminarCarrera).State = EntityState.Modified;

            try
            {
                 _context.Carreras.Remove(eliminarCarrera);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarreraExist(id))
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




        private bool CarreraExist(int id)
        {
            return _context.Carreras.Any(i => i.CarreraID == id);
        }
    }
}