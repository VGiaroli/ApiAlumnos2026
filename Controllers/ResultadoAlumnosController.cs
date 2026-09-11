using System;
using Microsoft.AspNetCore.Mvc;
using ApiAlumnos2026.ClasesVistasVarias;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace ApiAlumnos2026.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadoAlumnosController : ControllerBase
    {
        private readonly ApiAlumnos2026DbContext _context;

        public ResultadoAlumnosController(ApiAlumnos2026DbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ResultadoAlumnos>> GetResultadoAlumnos()
        {

            ResultadoAlumnos resultados = new ResultadoAlumnos();

            var sumNotas = _context.NotaAlumnos.Sum(n => n.Nota);
            var cantidadAlumnos = _context.NotaAlumnos.Count();

            resultados.Promedio = decimal.Round((decimal)sumNotas / cantidadAlumnos, 2);

            //NOTA MAS ALTA:
            //buscamos la nota mas alta
            var notaMasAlta = _context.NotaAlumnos.Max(n => n.Nota);
            resultados.NotaMasAlta = notaMasAlta;
            //buscamos la nota mas alta y el nombre del alumno que la obtuvo
            var alumnosNotaMasAlta = _context.NotaAlumnos
            .Include(n => n.Alumno)
            .Where(n => n.Nota == resultados.NotaMasAlta)
            .ToList();
            foreach (var alumnoNotaMasAlta in alumnosNotaMasAlta)
            {
                resultados.AlumnoNotaMasAlta += alumnoNotaMasAlta.Alumno.NombreCompleto;
            }


            //NOTA MAS BAJA:
            //buscamos la nota mas baja
            var notaMasBaja = _context.NotaAlumnos.Min(n => n.Nota);
            resultados.NotaMasBaja = notaMasBaja;
            //buscamos la nota mas baja 
            var alumnosNotaMasBaja = _context.NotaAlumnos
            // luego obtenemos todos los alumnos que tengan esa nota (puede haber más de uno)
            // incluimos Alumno para acceder a su NombreCompleto
            .Include(n => n.Alumno)
            .Where(n => n.Nota == resultados.NotaMasBaja)
            .ToList();
            foreach (var alumnoNotaMasBaja in alumnosNotaMasBaja)
            {
                resultados.AlumnoNotaMasBaja += alumnoNotaMasBaja.Alumno.NombreCompleto;
            }


            //CALCULAMOS LA CANTIDAD DE APROBADOS Y DESAPROBADOS
            //buscamos la cantidad de aprobados y desaprobados
            resultados.CantAprobados = _context.NotaAlumnos.Where(a => a.Nota >= 6).Count();

            resultados.CantDesaprobados = _context.NotaAlumnos.Where(d => d.Nota < 6).Count();

            //definimos el estado del grupo 
            if (resultados.Promedio >= 6)
            {
                resultados.EstadoDelGrupo = "Grupo aprobado";
            }
            else
            {
                resultados.EstadoDelGrupo = "Grupo en riesgo";
            }

            return resultados;

        }

    }
}