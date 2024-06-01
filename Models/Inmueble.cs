using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    public class Inmueble
    {
        [Key]
        public int IdInmueble { get; set; }

        public string? Direccion { get; set; }
        public string? Uso { get; set; }
        public string? Tipo { get; set; }
        public int Ambientes { get; set; }
        public int Superficie { get; set; }
        public decimal Latitud { get; set; }
        public decimal Valor { get; set; }
        public string? Imagen { get; set; }
        public bool? Disponible { get; set; }
        public decimal Longitud { get; set; }

        [ForeignKey("IdPropietario")]
        public int IdPropietario { get; set; }
        public Propietario? Duenio { get; set; }

        //[NotMapped]
      //  public byte[]? imagenfile { get; set; }

//        [NotMapped]
      //  public byte[]? pumba { get; set; }

       // [NotMapped]
        //public string? ImagenBase64 { get; set; }

        [NotMapped]
        public bool TieneContratoVigente { get; set; }  // Propiedad agregada

        public override string ToString()
        {
            return $"ID: {IdInmueble} / DIRECCION: {Direccion} / DUEÑO: {Duenio}";
        }
    }
}
