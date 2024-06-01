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
    public class ContratosController : Controller
    {
        private readonly DataContext contexto;

        public ContratosController(DataContext contexto)
        {
            this.contexto = contexto;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetContratosPropietario()
        {
            try
            {
                // Obtener el nombre de usuario del propietario logueado
                var usuario = User.Identity.Name;

                // Buscar el propietario logueado en la base de datos
                var propietario = await contexto.propietarios
                    .FirstOrDefaultAsync(p => p.Email == usuario);

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }

                // Obtener todos los inmuebles asociados al propietario
                var inmuebles = await contexto.Inmuebles
                    .Where(i => i.IdPropietario == propietario.IdPropietario)
                    .ToListAsync();

                // Lista para almacenar todos los contratos del propietario
                var contratosPropietario = new List<Contrato>();

                // Iterar sobre todos los inmuebles para obtener sus contratos
                foreach (var inmueble in inmuebles)
                {
                    // Obtener los contratos asociados a este inmueble, incluyendo los objetos Inquilino
                    var contratosInmueble = await contexto.contratos
                  .Include(c => c.Inquilino)
                  .Where(c => c.IdInmueble == inmueble.IdInmueble && c.Estado == true)
                  .ToListAsync();

                    // Agregar los contratos del inmueble a la lista de contratos del propietario
                    contratosPropietario.AddRange(contratosInmueble);
                }

                return Ok(contratosPropietario);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener los contratos: {ex.Message}");
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("inmueble/{id}")]
        public async Task<IActionResult> GetContratoPorInmueble(int id)
        {
            try
            {
                var usuario = User.Identity.Name;

                var propietario = await contexto.propietarios
                    .FirstOrDefaultAsync(p => p.Email == usuario);

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }

                var contrato = await contexto.contratos
                    .Include(c => c.Inquilino)
                    .Include(c => c.Inmueble) // Incluye el objeto Inmueble en la consulta
                    .FirstOrDefaultAsync(c => c.IdInmueble == id && c.Estado == true);

                if (contrato == null)
                {
                    return NotFound("Contrato no encontrado");
                }

                return Ok(contrato);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener el contrato: {ex.Message}");
            }
        }



    }
}
