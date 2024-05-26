using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    public class Contrato
    {
        [Key]
        public int IdContrato { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinalizacion { get; set; }
        public decimal MontoAlquiler { get; set; }
        public Boolean Estado { get; set; }
        [ForeignKey(nameof(Inquilino))]
        public int IdInquilino { get; set; }

        [ForeignKey(nameof(Inmueble))]
        public int IdInmueble { get; set; }

        public Inquilino? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }


        public override string ToString()
        {
            return $"Contrato: {IdContrato} Inquilino: {Inquilino} Inmueble: {Inmueble} /MONTO: {MontoAlquiler} ";
        }
    }
}