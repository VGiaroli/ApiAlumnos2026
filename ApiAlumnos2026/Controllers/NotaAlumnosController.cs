using ApiAlumnos2026.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

namespace ApiAlumnos2026.Controllers
{
    //TENER EN CUENTA PARA DESPUÉS PODER INICIAR EN SWAGGER
    [Route("api/[controller]")]
    [ApiController]
    public class NotaAlumnosController : ControllerBase
    {
        private readonly ApiAlumnos2026DbContext _context;

        public NotaAlumnosController(ApiAlumnos2026DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotaAlumno>>> GetNotaAlumnos()
        {
            return await _context.NotaAlumnos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotaAlumno>> GetNotaAlumnos(int id)
        {
            var notaAlumno = await _context.NotaAlumnos.FindAsync(id);

            if (notaAlumno == null)
                return NotFound();

            return notaAlumno;
        }

        //METODO CREAR------------------
        [HttpPost]
        public IActionResult AgregarNotaAlumno(NotaAlumno nuevaNota) //nuevaNota es un parametro dentro del método(NotaAlumno)
        {
            if (string.IsNullOrEmpty(nuevaNota.NombreAlumno))
                return BadRequest("El nombre del alumno no puede quedar vacio");

            //HasValue:
            //responde a si tiene un valor o está vacio
            //no debe ser 0 o igual a 0.
            // "||" significa "OR"
            if (!nuevaNota.Nota.HasValue || nuevaNota.Nota <= 0)
                return BadRequest("No puede quedar vacio, debe ingresar una nota");

            if (!nuevaNota.NumeroDNI.HasValue)
                return BadRequest("No puede quedar vacio, debe ingresar el numero de DNI");

            // usamos bool para verificar si ya existe un alumno con el mismo nombre y DNI.
            // con el Any() devuelve true si al menos un registro cumple la condición especificada.
            // devuelve un true o false, dependiendo si ya hay un alumno y DNI cargado.
            bool alumnoExiste = _context.NotaAlumnos
                .Any(n => n.NombreAlumno == nuevaNota.NombreAlumno
                && n.NumeroDNI == nuevaNota.NumeroDNI);

            bool dniExiste = _context.NotaAlumnos
                .Any(n => n.NumeroDNI == nuevaNota.NumeroDNI);

            //VALIDACIONES.
            // Lo que indica ModelState es que si hay algun error, que no agregue nada y muestre otra vez el formulario
            if (alumnoExiste)
                ModelState.AddModelError("Duplicado", "Ya existe un alumno con ese nombre y DNI.");

            if (dniExiste)
                ModelState.AddModelError("NumeroDNI", "Ya existe un alumno registrado con ese DNI.");

            //Aca pregunta, Hay errores?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //si esta bien lo guarda en la base de datos
            _context.NotaAlumnos.Add(nuevaNota);
            _context.SaveChanges();


            return Ok(nuevaNota);
        }

        //METODO EDITAR------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, NotaAlumno notaAlumno)
        {
            //compara el id que viene de la URL, con el id del objeto que viene del body
            if (id != notaAlumno.NotaAlumnoId)
            {
                return BadRequest();
            }

            bool alumnoExiste = _context.NotaAlumnos
                .Any(n => n.NombreAlumno == notaAlumno.NombreAlumno
                && n.NumeroDNI == notaAlumno.NumeroDNI
                && n.NotaAlumnoId != id);

            bool dniExiste = _context.NotaAlumnos
                .Any(n => n.NumeroDNI == notaAlumno.NumeroDNI
                && n.NotaAlumnoId != id);

            if (alumnoExiste)
                ModelState.AddModelError("Duplicado", "Ya existe un alumno con ese nombre y DNI.");

            if (dniExiste)
                ModelState.AddModelError("NumeroDNI", "Ya existe un alumno registrado con ese DNI.");
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //aca entity framework sabe que el objeto esta en la base de datos, pero tambien sabe que se modifico
            _context.Entry(notaAlumno).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //Si el método NotaAlumnoExist(id) devuelve falso, 
                //significa que el registro ya no existe, por lo que responde un 404
                if (!NotaAlumnoExist(id))
                {
                    return NotFound();
                }
                // Si el registro existe pero hubo otro error de base de datos, usaremos throw
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //METODO ELIMINAR------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotaAlumno(int id)
        {
            var notaAlumno = await _context.NotaAlumnos.FindAsync(id);
            if (notaAlumno == null)
            {
                return NotFound();
            }

            try
            {
                _context.NotaAlumnos.Remove(notaAlumno);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotaAlumnoExist(id))
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

        private bool NotaAlumnoExist(int id)
        {
            return _context.NotaAlumnos.Any(x => x.NotaAlumnoId == id);
        }
    }
}