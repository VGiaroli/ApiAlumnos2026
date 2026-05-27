using System.Threading.Tasks;
using ApiAlumnos2026.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using NuGet.Common;

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
            var notas = await _context.NotaAlumnos
            .Select(n => new
            {
                //propiedades básicas
                n.NotaAlumnoId,
                n.Nota,
                n.AlumnoId,
                n.AsignaturaId,
                n.Fecha,
                //traigo solo los nombres, no el objeto completo
                //solamente le estoy pidiendo esa información
                NombreAlumno = n.Alumno!.NombreCompleto,
                NombreAsignatura = n.Asignatura!.Descripcion
            })
            .ToListAsync();

            return Ok(notas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotaAlumno>> GetNotaAlumnos(int id)
        {
            var notaAlumno = await _context.NotaAlumnos
            .Select(n => new
            {
                //propiedades básicas
                n.NotaAlumnoId,
                n.Nota,
                n.AlumnoId,
                n.AsignaturaId,
                n.Fecha,
                //traigo solo los nombres, no el objeto completo
                //solamente le estoy pidiendo esa información
                NombreAlumno = n.Alumno!.NombreCompleto,
                NombreAsignatura = n.Asignatura!.Descripcion
            })
            .FirstOrDefaultAsync(n => n.NotaAlumnoId == id);
            //no debo traer una lista completa, sino el primero y el correcto

            if (notaAlumno == null)
            {
                return NotFound();
            }

            return Ok(notaAlumno);
        }

        //METODO CREAR------------------
        [HttpPost]
        //devuelve uno solo <IActionResult>, si trabaja con mas deberiamos trabajar con IEnumerable
        public async Task<IActionResult> AgregarNotaAlumno([FromBody] NotaAlumno nuevaNota) //nuevaNota es un parametro dentro del método(NotaAlumno)
        {
            //HACER OTRA VEZ DE CERO EL OBJETO PARA CONTROLAR LOS DATOS QUE QUEREMOS GUARDAR
            if (nuevaNota == null)
            {
                return BadRequest();
            }

            //falta hacer validaciones AsignaturaId y AlumnoId, Nota y Fecha

            var guardarNotaAlumno = new NotaAlumno
            {
                Nota = nuevaNota.Nota,
                AlumnoId = nuevaNota.AlumnoId,
                AsignaturaId = nuevaNota.AsignaturaId,
                Fecha = nuevaNota.Fecha
            };

            _context.NotaAlumnos.Add(guardarNotaAlumno);
            await _context.SaveChangesAsync();

            return Ok(guardarNotaAlumno);
        }

        //METODO EDITAR------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, NotaAlumno notaAlumno)
        {
            if (id != notaAlumno.NotaAlumnoId)
            {
                return BadRequest("ID inválido");
            }

            try
            {
                // BUSCAR REGISTRO ORIGINAL
                var notaOriginalAlumno = await _context.NotaAlumnos
                    .Include(n => n.Alumno)
                    .Include(n => n.Asignatura)
                    .FirstOrDefaultAsync(n => n.NotaAlumnoId == id);

                if (notaOriginalAlumno == null)
                {
                    return NotFound("Nota no encontrada");
                }

                // ============================
                // FECHA
                // ============================
                if (notaOriginalAlumno.Fecha != notaAlumno.Fecha)
                {
                    _context.HistorialNotaAlumnos.Add(new HistorialNotaAlumno
                    {
                        NotaAlumnoID = id,
                        FechaCambio = DateTime.Now,
                        CampoModificado = "FECHA",
                        ValorAnterior = notaOriginalAlumno.Fecha.ToString("dd/MM/yyyy"),
                        ValorNuevo = notaAlumno.Fecha.ToString("dd/MM/yyyy")
                    });
                }

                // ============================
                // ALUMNO
                // ============================
                if (notaOriginalAlumno.AlumnoId != notaAlumno.AlumnoId)
                {
                    var alumnoNuevo = await _context.Alumnos
                        .FirstOrDefaultAsync(a => a.AlumnoId == notaAlumno.AlumnoId);

                    if (alumnoNuevo == null)
                    {
                        return BadRequest("Alumno no encontrado");
                    }

                    _context.HistorialNotaAlumnos.Add(new HistorialNotaAlumno
                    {
                        NotaAlumnoID = id,
                        FechaCambio = DateTime.Now,
                        CampoModificado = "ALUMNO",
                        ValorAnterior = notaOriginalAlumno.Alumno.NombreCompleto,
                        ValorNuevo = alumnoNuevo.NombreCompleto
                    });
                }

                // ============================
                // ASIGNATURA
                // ============================
                if (notaOriginalAlumno.AsignaturaId != notaAlumno.AsignaturaId)
                {
                    var asignaturaNueva = await _context.Asignaturas
                        .FirstOrDefaultAsync(a => a.AsignaturaId == notaAlumno.AsignaturaId);

                    if (asignaturaNueva == null)
                    {
                        return BadRequest("Asignatura no encontrada");
                    }

                    _context.HistorialNotaAlumnos.Add(new HistorialNotaAlumno
                    {
                        NotaAlumnoID = id,
                        FechaCambio = DateTime.Now,
                        CampoModificado = "ASIGNATURA",
                        ValorAnterior = notaOriginalAlumno.Asignatura.Descripcion,
                        ValorNuevo = asignaturaNueva.Descripcion
                    });
                }

                // ============================
                // NOTA
                // ============================
                if (notaOriginalAlumno.Nota != notaAlumno.Nota)
                {
                    _context.HistorialNotaAlumnos.Add(new HistorialNotaAlumno
                    {
                        NotaAlumnoID = id,
                        FechaCambio = DateTime.Now,
                        CampoModificado = "NOTA",
                        ValorAnterior = notaOriginalAlumno.Nota.ToString(),
                        ValorNuevo = notaAlumno.Nota.ToString()
                    });
                }

                // ============================
                // ACTUALIZAR REGISTRO
                // ============================
                notaOriginalAlumno.Nota = notaAlumno.Nota;
                notaOriginalAlumno.AlumnoId = notaAlumno.AlumnoId;
                notaOriginalAlumno.AsignaturaId = notaAlumno.AsignaturaId;
                notaOriginalAlumno.Fecha = notaAlumno.Fecha;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<NotaAlumno>>> AsignarDatos()
        // {
        //     var obtenerAlumnos = await _context.NotaAlumnos.Include(n => n.Alumno).ToListAsync();

        //     foreach (var obtenerAlumno in obtenerAlumnos)
        //     {
        //         var alumnos = _context.Alumnos
        //             .Where(d => d.DNI == obtenerAlumno.Alumno.DNI).SingleOrDefault();

        //         if(alumnos == null)
        //         {
        //             alumnos = new Alumno
        //             {
        //                 NombreCompleto = obtenerAlumno.Alumno.NombreCompleto,
        //                 Domicilio = "",
        //                 Sexo = Sexo.Otro,
        //                 DNI = obtenerAlumno.Alumno.DNI
        //             };

        //             _context.Alumnos.Add(alumnos);
        //             await _context.SaveChangesAsync();
        //         }
        //     }

        //     return obtenerAlumnos;
        // }

        private bool NotaAlumnoExist(int id)
        {
            return _context.NotaAlumnos.Any(x => x.NotaAlumnoId == id);
        }
    }
}