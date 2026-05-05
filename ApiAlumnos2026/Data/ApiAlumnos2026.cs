using System;
using Microsoft.EntityFrameworkCore;
using ApiAlumnos2026.Models;
//El archivo le dice al programa que COMO debe conectarse a la BD y que tablas existen.
//Esta clase maneja la BD
public class ApiAlumnos2026DbContext : DbContext
{
    //Le dice como conectarse a la BD (servidor, nombre, etc).
    public ApiAlumnos2026DbContext(DbContextOptions<ApiAlumnos2026DbContext> options)
        : base(options)
    {
        
    }

    // DbSet<NotaAlumno> → representa una tabla.
    // NotaAlumnos → nombre de la tabla en el código.
    public DbSet<NotaAlumno> NotaAlumnos { get; set; }

    public DbSet<Asignatura> Asignaturas { get; set; }

    public DbSet<Alumno> Alumnos { get; set; }

    public DbSet<Docente> Docentes { get; set; }
    
}

//Tengo una base de datos, y dentro una tabla llamada NotaAlumnos, donde
// guardo objeto del tipo NotaAlumno.