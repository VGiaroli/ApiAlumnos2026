namespace ApiAlumnos2026.Models;

public class Docente
{
    public int DocenteId { get; set; }

    public string NombreCompleto { get; set; }

    public int DNI { get; set; }

    public string? Email {get; set;}

    public Sexo Sexo {get; set;}   

}

public enum Sexo
{
    Hombre = 1,
    Mujer,
    Otro
}