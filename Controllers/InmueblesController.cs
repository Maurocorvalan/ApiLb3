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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiLb3.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]

    public class InmueblesController : Controller
    {
        private readonly DataContext contexto;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public InmueblesController(IWebHostEnvironment webHostEnvironment, DataContext contexto)
        {
            _webHostEnvironment = webHostEnvironment;
            this.contexto = contexto;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                Console.WriteLine(usuario);

                var propietario = await contexto.propietarios.FirstOrDefaultAsync(p => p.Email == usuario);
                Console.WriteLine(propietario.IdPropietario);

                if (propietario != null)
                {
                    var inmuebles = await contexto.Inmuebles
                        .Where(i => i.IdPropietario == propietario.IdPropietario)
                        .ToListAsync();
                    Console.WriteLine(inmuebles);
                    return Ok(inmuebles);
                }
                else
                {
                    return NotFound("Propietario no encontrado");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        //get para traer inmueble si posee contrato vigente

        [HttpGet("GetContratoVigente")]
        public async Task<IActionResult> GetContratoVigente()
        {
            try
            {
                var usuario = User.Identity.Name;
                Console.WriteLine(usuario);

                var propietario = await contexto.propietarios.FirstOrDefaultAsync(p => p.Email == usuario);
                if (propietario != null)
                {
                    var inmuebles = await contexto.Inmuebles
                        .Where(i => i.IdPropietario == propietario.IdPropietario)
                        .Select(i => new
                        {
                            i.IdInmueble,
                            i.Direccion,
                            i.Valor,
                            i.IdPropietario,
                            i.Uso,
                            i.Tipo,
                            i.Ambientes,
                            i.Superficie,
                            i.Latitud,
                            i.Longitud,
                            i.Imagen,
                            i.Disponible,
                            TieneContratoVigente = contexto.contratos.Any(c => c.IdInmueble == i.IdInmueble && c.FechaFinalizacion > DateTime.Now)
                        })
                        .Where(i => i.TieneContratoVigente) // Filtrar solo inmuebles con contratos vigentes
                        .ToListAsync();

                    Console.WriteLine(inmuebles);
                    return Ok(inmuebles);
                }
                else
                {
                    return NotFound("Propietario no encontrado");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("cargar")]
        public async Task<IActionResult> Cargar([FromForm] IFormFile imagen, [FromForm] string inmueble)
        {
            Console.WriteLine("estas aca");
            try
            {
                // Deserializa el JSON al objeto Inmueble
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var inmuebleObject = JsonSerializer.Deserialize<Inmueble>(inmueble, options);

                Console.WriteLine(inmuebleObject.ToString());

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (imagen != null)
                {
                    var uploadsRootFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsRootFolder))
                    {
                        Directory.CreateDirectory(uploadsRootFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                    var filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imagen.CopyToAsync(fileStream);
                    }

                    inmuebleObject.Imagen = Path.Combine("uploads", uniqueFileName);
                }

                var user = HttpContext.User;
                var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(userIdClaim, out int propietarioId))
                {
                    inmuebleObject.IdPropietario = propietarioId;
                    contexto.Inmuebles.Add(inmuebleObject);
                    await contexto.SaveChangesAsync();
                    return CreatedAtAction(nameof(Get), new { id = inmuebleObject.IdInmueble }, inmuebleObject);
                }
                else
                {
                    return BadRequest("Invalid user ID format in token");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpDelete("Delete/{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var inmueble = await contexto.Inmuebles.FindAsync(id);
                if (inmueble == null)
                {
                    return NotFound("Inmueble no encontrado");
                }

                contexto.Inmuebles.Remove(inmueble);
                await contexto.SaveChangesAsync();

                return Ok("Inmueble eliminado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + " Inmueble no pertenece a propietario");
            }
        }




        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Inmueble entidad)
        {
            try
            {
                if (id != entidad.IdInmueble)
                {
                    return BadRequest("El ID del inmueble en la URL no coincide con el ID del inmueble proporcionado en el cuerpo de la solicitud.");
                }

                var inmuebleExistente = await contexto.Inmuebles.FindAsync(id);
                if (inmuebleExistente == null)
                {
                    return NotFound("Inmueble no encontrado");
                }

                // Desconecta la entidad rastreada
                contexto.Entry(inmuebleExistente).State = EntityState.Detached;

                entidad.IdInmueble = id;
                contexto.Update(entidad);
                await contexto.SaveChangesAsync();

                return Ok("Inmueble actualizado correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar el inmueble: " + ex.Message);
            }
        }

        //solo actualiza el estado de disponibilidad.
        [HttpPut("actualizar")]
        public async Task<IActionResult> Put([FromBody] Inmueble entidad)
        {
            try
            {
                Console.WriteLine("entro aca");
                if (entidad == null)
                {
                    return BadRequest("El objeto de inmueble proporcionado es nulo.");
                }

                var inmuebleExistente = await contexto.Inmuebles.FindAsync(entidad.IdInmueble);

                if (inmuebleExistente == null)
                {
                    return NotFound("Inmueble no encontrado");
                }

                // Actualiza solo el estado de la propiedad 'disponible' del inmueble existente
                inmuebleExistente.Disponible = entidad.Disponible;

                await contexto.SaveChangesAsync();

                return Ok(inmuebleExistente);
            }
            catch (Exception ex)
            {
                return BadRequest("Error al actualizar el inmueble: " + ex.Message);
            }
        }


    }

}