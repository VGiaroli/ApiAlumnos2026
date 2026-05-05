using ApiAlumnos2026.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAlumnos2026.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {
        private readonly ApiAlumnos2026DbContext _context;

        public AlumnosController(ApiAlumnos2026DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alumno>>> GetAlumnos()
        {
            return await _context.Alumnos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Alumno>> GetObtenerAlumnos(int id)
        {
            var obtenerAlumno = await _context.Alumnos.FindAsync(id);
            if (obtenerAlumno == null)
            {
                return NotFound();
            }

            return obtenerAlumno;
        }

        [HttpPost]
        public async Task<ActionResult<Alumno>> PostAlumnos([FromBody] Alumno nuevoAlumno)
        {
            if(nuevoAlumno.NombreCompleto == "")
            {
                return BadRequest("El nombre completo es obligatorio.");
            }

            if(nuevoAlumno.Domicilio == "")
            {
                return BadRequest("El domicilio es obligatorio.");
            }

            // validacion para verificar que el DNI sea un número válido de 8 dígitos
            if(nuevoAlumno.DNI < 10000000 || nuevoAlumno.DNI > 99999999)
            {
                return BadRequest("El DNI es obligatorio y debe ser un número válido.");
            }

            bool existeDNI = _context.Alumnos.Any(a => a.DNI == nuevoAlumno.DNI);
            if(existeDNI)
            {
                return BadRequest("El DNI le pertenece a otro alumno.");
            }

            if(nuevoAlumno.Sexo == null)
            {
                return BadRequest("El sexo es obligatorio.");
            }


             // Si hay errores de validación, ModelState.IsValid será falso

            // Si hay errores de conversión, ModelState.IsValid será falso
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Alumnos.Add(nuevoAlumno);
            await _context.SaveChangesAsync();
            return Ok(nuevoAlumno);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditAlumno(int id, [FromBody] Alumno editarAlumno)
        {

            if(editarAlumno.NombreCompleto == null)
            {
                return BadRequest("El nombre completo es obligatorio.");
            }

            if(editarAlumno.Domicilio == null)
            {
                return BadRequest("El domicilio es obligatorio.");
            }
            
            // validacion para verificar que el DNI no se repita en otro docente
            // después del && me esta diciendo:
            // si este DNI ya existe, pero ignorá al alumno que estoy editando ahora mismo. 
            // solo devolveme true si otro alumno distinto ya está usando este DNI
            bool existeDNI = _context.Alumnos.Any(a => a.DNI == editarAlumno.DNI && a.AlumnoId != id);
            if(existeDNI)
            {
                return BadRequest("El DNI le pertenece a otro alumno.");
            }

            if(editarAlumno.Sexo == null)
            {
                return BadRequest("El sexo es obligatorio.");
            }

             if(editarAlumno.DNI == 0)
            {
                return BadRequest("El DNI es obligatorio y debe ser un número válido.");
            }

            var editAlumno = new Alumno
            {
                AlumnoId = id,
                NombreCompleto = editarAlumno.NombreCompleto,
                DNI = editarAlumno.DNI,
                Domicilio = editarAlumno.Domicilio,
                Sexo = editarAlumno.Sexo
            };

            _context.Entry(editAlumno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlumnosExists(editarAlumno.AlumnoId))
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
        public async Task<IActionResult> DeleteAlumno(int id)
        {
            var deleteAlumno = await _context.Alumnos.FindAsync(id);
            if (deleteAlumno == null)
            {
                return NotFound();
            }

            try
            {
                _context.Alumnos.Remove(deleteAlumno);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlumnosExists(id))
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

        private bool AlumnosExists(int id)
        {
            return _context.Alumnos.Any(a => a.AlumnoId == id);
        }
    }
}