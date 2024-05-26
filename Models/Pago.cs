using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria.Models
{
    public class Pago
    {       
        [Key]
        public int IdPago { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string? Detalle { get; set; }
        public bool Estado { get; set; }

        [ForeignKey(nameof(Contrato))]
        public int IdContrato { get; set; }
        public Contrato? Contrato { get; set; }

        public override string ToString()
        {
            return $"ID: {IdPago}  /FECHA: {FechaPago} /MONTO: {Monto}";
        }
    }
}
