using System.Security.Claims;
using ApiAlumnos2026.ClasesVistasVarias;
using ApiAlumnos2026.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAlumnos2026.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocentesController : ControllerBase
    {
        private readonly ApiAlumnos2026DbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocentesController(ApiAlumnos2026DbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Docente>>> GetDocente()
        {
            List<Docente> vistaDocentes = new List<Docente>();

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
                        var asignaturasDocente = _context.Docentes.Where(a => a.DocenteId == docente.DocenteId).ToList();

                        foreach (var asignaturaDocente in asignaturasDocente)
                        {
                            var asignatura = _context.Docentes.Where(a => a.DocenteId == asignaturaDocente.DocenteId).Single();

                            var elemento = new Docente
                            {
                                DocenteId = asignatura.DocenteId,
                                NombreCompleto = asignatura.NombreCompleto,
                                Sexo = asignatura.Sexo,
                                Email = asignatura.Email,
                                DNI = asignatura.DNI
                            };
                            vistaDocentes.Add(elemento);
                        }
                    }
                }
                else
                {
                    var asignaturas = await _context.Docentes.OrderBy(n => n.NombreCompleto).ToListAsync();

                    foreach (var asignatura in asignaturas)
                    {
                        var elemento = new Docente
                        {
                            DocenteId = asignatura.DocenteId,
                            NombreCompleto = asignatura.NombreCompleto,
                            Sexo = asignatura.Sexo,
                            Email = asignatura.Email,
                            DNI = asignatura.DNI
                        };
                        vistaDocentes.Add(elemento);

                    }
                }
            }

            return vistaDocentes;
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

            if (nuevoDocente.NombreCompleto == null)
            {
                return BadRequest("El nombre completo es obligatorio.");
            }
            //validacion para verificar que el DNI sea un número válido de 8 dígitos
            if (nuevoDocente.DNI < 10000000 || nuevoDocente.DNI > 99999999)
            {
                return BadRequest("El DNI es obligatorio y debe ser un número válido.");
            }

            _context.Docentes.Add(nuevoDocente);

            var user = new ApplicationUser
            {
                UserName = nuevoDocente.Email,
                Email = nuevoDocente.Email,
                NombreCompleto = nuevoDocente.NombreCompleto
            };

            var result = await _userManager.CreateAsync(user, "Ezpeleta_2026");

            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBuscarDocentes", new { id = nuevoDocente.DocenteId }, nuevoDocente);
            }

            var error = "";
            foreach (var textoerror in result.Errors)
            {
                error += textoerror.Description;
            }

            return BadRequest(new
            {
                mensaje = error
            });

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditDocente(int Id, Docente editarDocente)
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

            if (editarDocente.NombreCompleto == null)
            {
                return BadRequest("El nombre completo es obligatorio.");
            }

            if (editarDocente.DNI == 0)
            {
                return BadRequest("El DNI es obligatorio y debe ser un número válido.");
            }

            if (editarDocente.Sexo == null)
            {
                return BadRequest("El sexo es obligatorio.");
            }

            try
            {
                var docenteOriginal = await _context.Docentes
                .FirstOrDefaultAsync(d => d.DocenteId == Id);

                if (docenteOriginal == null)
                {
                    return NotFound("Docente no encontrado");
                }

                //DOCENTE
                if (docenteOriginal.DocenteId != editarDocente.DocenteId)
                {

                    var nuevoDocente = await _context.Docentes
                        .FirstOrDefaultAsync(n => n.DocenteId == editarDocente.DocenteId);

                    if (nuevoDocente == null)
                    {
                        return BadRequest("Docente no encontrado");
                    }

                    _context.HistorialDocentes.Add(new HistorialDocente
                    {
                        DocenteId = Id,
                        FechaCambio = DateTime.Now,
                        CampoModificado = "DOCENTE",
                        ValorAnterior = docenteOriginal.DocenteId.ToString(),
                        ValorNuevo = editarDocente.DocenteId.ToString()
                    });
                }

                //NOMBRE COMPLETO
                if (docenteOriginal.NombreCompleto != editarDocente.NombreCompleto)
                {
                    _context.HistorialDocentes.Add(new HistorialDocente
                    {
                        DocenteId = Id,
                        FechaCambio = DateTime.Now,
                        CampoModificado = "NOMBRE COMPLETO",
                        ValorAnterior = docenteOriginal.NombreCompleto.ToString(),
                        ValorNuevo = editarDocente.NombreCompleto.ToString()
                    });
                }

                //NUMERO DE DNI
                if (docenteOriginal.DNI != editarDocente.DNI)
                {
                    _context.HistorialDocentes.Add(new HistorialDocente
                    {
                        DocenteId = Id,
                        FechaCambio = DateTime.Now,
                        CampoModificado = "DNI",
                        ValorAnterior = docenteOriginal.DNI.ToString(),
                        ValorNuevo = editarDocente.DNI.ToString()
                    });
                }

                //SEXO
                if (docenteOriginal.Sexo != editarDocente.Sexo)
                {
                    _context.HistorialDocentes.Add(new HistorialDocente
                    {
                        DocenteId = Id,
                        FechaCambio = DateTime.Now,
                        CampoModificado = "SEXO",
                        ValorAnterior = docenteOriginal.Sexo.ToString(),
                        ValorNuevo = editarDocente.Sexo.ToString()
                    });
                }


                docenteOriginal.DocenteId = editarDocente.DocenteId;
                docenteOriginal.Sexo = editarDocente.Sexo;
                docenteOriginal.NombreCompleto = editarDocente.NombreCompleto;
                docenteOriginal.DNI = editarDocente.DNI;


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




        //FUNCIONES PARA GUARDAR ASIGNATURA DOCENTE Y ELIMINAR LA ASIGNACION
        [HttpGet("AsignaturasDocente/{id}")]
        public async Task<ActionResult<IEnumerable<VistaAsignaturaDocente>>> MostrarAsignaturasDocente(int id)
        {
            List<VistaAsignaturaDocente> asignaturasMostrar = new List<VistaAsignaturaDocente>();

            var asignaturasDocente = await _context.AsignaturaDocentes
            .Where(a => a.DocenteId == id)
            .ToListAsync();

            foreach (var asignaturaDocente in asignaturasDocente)
            {
                var asignatura = _context.Asignaturas.Where(a => a.AsignaturaId == asignaturaDocente.AsignaturaId).Single();
                var asignaturaMostrar = new VistaAsignaturaDocente
                {
                    AsignaturaDocenteId = asignaturaDocente.AsignaturaDocenteId,
                    AsignaturaId = asignaturaDocente.AsignaturaId,
                    Descripcion = asignatura.Descripcion
                };
                asignaturasMostrar.Add(asignaturaMostrar);
            }

            return asignaturasMostrar.ToList();
        }

        [HttpPost("GuardaAsignaturasDocente")]
        public async Task<ActionResult<AsignaturaDocente>> GuardarAsignaturas(AsignaturaDocente asignaturaDocente)
        {
            if (asignaturaDocente.DocenteId > 0 && asignaturaDocente.AsignaturaId > 0)
            {
                var existeAsignaturaDocente = _context.AsignaturaDocentes
                    .Where(a => a.DocenteId == asignaturaDocente.DocenteId
                    && a.AsignaturaId == asignaturaDocente.AsignaturaId).Count();

                if (existeAsignaturaDocente == 0)
                {
                    _context.AsignaturaDocentes.Add(asignaturaDocente);
                    _context.SaveChanges();
                }
            }
            return asignaturaDocente;
        }

        [HttpDelete("EliminaAsignaturasDocente/{id}")]
        public async Task<IActionResult> EliminarAsignatura(int id)
        {
            var asigDocente = await _context.AsignaturaDocentes.FindAsync(id);
            if (asigDocente == null)
            {
                return NotFound();
            }

            _context.AsignaturaDocentes.Remove(asigDocente);
            await _context.SaveChangesAsync();

            return Ok();
        }



        private bool DocenteExist(int id)
        {
            return _context.Docentes.Any(d => d.DocenteId == id);
        }
    }
}