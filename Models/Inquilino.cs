using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria.Models;

public class Inquilino
{
    [Key]
    public int IdInquilino { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }

    public string? Dni { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public override string ToString()
    {
        return $"{Nombre} {Apellido} {Dni}";
    }
}
