using ApiAlumnos2026.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAlumnos2026.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocentesController : ControllerBase
    {
        private readonly ApiAlumnos2026DbContext _context;

        public DocentesController(ApiAlumnos2026DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Docente>>> GetDocentes()
        {
            return await _context.Docentes.ToListAsync();
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<Docente>> GetBuscarDocentes(int id)
        {
            var obtenerDocente = await _context.Docentes.FindAsync(id);

            if (obtenerDocente == null)
            {
                return NotFound();
            }

            return obtenerDocente;
        }

        [HttpPost]
        public async Task<ActionResult<Docente>> PostDocente([FromBody] Docente nuevoDocente) // Cambiado a nuevoDocente
        {
            //validacion para verificar que el DNI no se repita en otro docente
            //antes de agregar el nuevo docente, verifico si ya existe otro docente con el mismo DNI
            bool existeDNI = _context.Docentes.Any(d => d.DNI == nuevoDocente.DNI);
            if (existeDNI)
            {
                return BadRequest("El DNI le pertenece a otro docente.");
            }

            if(nuevoDocente.NombreCompleto == null)
            {
                return BadRequest("El nombre completo es obligatorio.");
            }
            //validacion para verificar que el DNI sea un número válido de 8 dígitos
            if(nuevoDocente.DNI < 10000000 || nuevoDocente.DNI > 99999999)
            {
                return BadRequest("El DNI es obligatorio y debe ser un número válido.");
            }

            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Docentes.Add(nuevoDocente);
            await _context.SaveChangesAsync();
            return Ok(nuevoDocente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditDocente(int Id, [FromBody] Docente editarDocente)
        {

            // validacion para verificar que el DNI no se repita en otro docente
            // después del && me esta diciendo:
            // si este DNI ya existe, pero ignorá al docente que estoy editando ahora mismo. 
            // solo devolveme true si otro docente distinto ya está usando este DNI
            bool existeDNI = _context.Docentes.Any(d => d.DNI == editarDocente.DNI && d.DocenteId != Id);
            if (existeDNI)
            {
                return BadRequest("El DNI le pertenece a otro docente.");
            }

            if (Id != editarDocente.DocenteId)
            {
                return BadRequest("El Id del Docente no coincide.");
            }

            if(editarDocente.NombreCompleto == null)
            {
                return BadRequest("El nombre completo es obligatorio.");
            }

            if(editarDocente.DNI == 0)
            {
                return BadRequest("El DNI es obligatorio y debe ser un número válido.");
            }

            if(editarDocente.Sexo == null)
            {
                return BadRequest("El sexo es obligatorio.");
            }

            var docenteEditar = new Docente
            {
                DocenteId = editarDocente.DocenteId,
                NombreCompleto = editarDocente.NombreCompleto,
                DNI = editarDocente.DNI,
                Sexo = editarDocente.Sexo
            };

            _context.Entry(docenteEditar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocenteExist(editarDocente.DocenteId))
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
        public async Task<IActionResult> DeleteDocente(int id)
        {
            var eliminarDocente = await _context.Docentes.FindAsync(id);
            if (eliminarDocente == null)
            {
                return NotFound();
            }

            try
            {
                _context.Docentes.Remove(eliminarDocente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocenteExist(id))
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


        private bool DocenteExist(int id)
        {
            return _context.Docentes.Any(d => d.DocenteId == id);
        }
    }
}