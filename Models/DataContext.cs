using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ApiLb3.Models;
using Inmobiliaria.Models;
namespace ApiLb3.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Propietario> propietarios { get; set; }

        public DbSet<Inmueble> Inmuebles { get; set; }
        public DbSet<Contrato> contratos { get; set; }
        public DbSet<Inquilino> inquilinos { get; set; }

        public DbSet<Pago> pagos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la convención de nombres para la relación entre Inmueble y Propietario
            modelBuilder.Entity<Inmueble>()
                .HasOne(i => i.Duenio)
                .WithMany()
                .HasForeignKey(i => i.IdPropietario)
                .HasPrincipalKey(p => p.IdPropietario);
            modelBuilder.Entity<Inquilino>()
            .ToTable("inquilinos");
            base.OnModelCreating(modelBuilder);
        }


    }
}