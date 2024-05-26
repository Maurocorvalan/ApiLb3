using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Inmobiliaria.Models;

public class Usuario{
	public enum enRoles
	{
		Administrador = 1,
		Empleado = 2,
	}    public int IdUsuario {get; set;}
    public string? Nombre {get; set;}
    public string? Apellido{get;set;}
    public string? Email{get;set;}

    public string? Clave{get;set;}

    public string? AvatarUrl{get;set;}
    public IFormFile? AvatarFile{get;set;}
    public int Rol{get;set;}


public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

		public static IDictionary<int, string> ObtenerRoles()
		{
			SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
			Type tipoEnumRol = typeof(enRoles);
			foreach (var valor in Enum.GetValues(tipoEnumRol))
			{
				roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
			}
			return roles;
		}

		    public override string ToString()
    {
        return $"{Nombre} {Apellido} {Email}";
    }
	}
