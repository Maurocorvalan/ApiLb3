using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiLb3.Models;
using Inmobiliaria.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MimeKit;

namespace ApiLb3.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PagosController : Controller
    {
        private readonly DataContext contexto;

        public PagosController(DataContext contexto)
        {
            this.contexto = contexto;
        }

        [HttpGet("contrato/{id}")]
        public async Task<IActionResult> GetPagosPorContrato(int id)
        {
            try
            {
                // Imprimir el ID del contrato por consola
                Console.WriteLine($"ID del contrato: {id}");

                // Buscar los pagos asociados al contrato en la base de datos,
                // incluyendo la relación con el contrato
                var pagos = await contexto.pagos
                    .Include(p => p.Contrato) // Incluir el contrato asociado al pago
                    .Where(p => p.IdContrato == id)
                    .ToListAsync();

                if (pagos == null || pagos.Count == 0)
                {
                    return NotFound("No se encontraron pagos para este contrato");
                }

                return Ok(pagos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener los pagos del contrato: {ex.Message}");
            }
        }





    }
}