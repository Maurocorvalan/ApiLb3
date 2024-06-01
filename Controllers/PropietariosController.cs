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
using MimeKit.Cryptography;
using MailKit;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiLb3.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PropietariosController : ControllerBase  //
    {
        private readonly DataContext contexto;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;


        public PropietariosController(DataContext contexto, IConfiguration configuration, IWebHostEnvironment env)
        {
            this.contexto = contexto;
            this.configuration = configuration;
            environment = env;

        }
        // GET: api/<controller>
        [HttpGet]
        public async Task<ActionResult<Propietario>> Get()
        {
            try
            {
                var usuario = User.Identity.Name;
                return await contexto.propietarios.SingleOrDefaultAsync(x => x.Email == usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET api/<controller>/token
        [HttpGet("token")]
        public IActionResult Token()
        {
            try
            {
                var perfil = new
                {
                    Email = User.Identity.Name,
                    Nombre = User.Claims.First(x => x.Type == "FullName").Value,
                    Rol = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value,
                    Dni = User.Claims.FirstOrDefault(x => x.Type == "Dni")?.Value,
                    Telefono = User.Claims.FirstOrDefault(x => x.Type == "Telefono")?.Value,


                };

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET api/<controller>/email[HttpPost("email")]
        [HttpPost("email")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByEmailes([FromForm] string email)
        {
            try
            {
                // Buscar el propietario por email
                var entidad = await contexto.propietarios.FirstOrDefaultAsync(x => x.Email == email);
                if (entidad == null)
                {
                    return BadRequest("El email ingresado no existe.");
                }
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                var result = new StringBuilder(5);
                Random random = new Random();

                for (int i = 0; i < 6; i++)
                {
                    result.Append(chars[random.Next(chars.Length)]);
                }

                String enlace = result.ToString();
                // Enviar el correo electrónico
                string token = HashPassword(enlace);
                Console.WriteLine("Contraseña del email: " + token);
                Console.WriteLine("Enlace del email: " + enlace);
                entidad.Clave = token;
                Console.WriteLine(token);
                contexto.propietarios.Update(entidad);
                await contexto.SaveChangesAsync();
                var message = new MimeKit.MimeMessage();
                message.To.Add(new MailboxAddress(entidad.Nombre, entidad.Email));
                message.From.Add(new MailboxAddress("Sistema", configuration["Smtp:User"]));
                message.Subject = "Restablecer contraseña";
                message.Body = new TextPart("html")
                {
                    Text = $@"<p>Hola {entidad.Nombre}:</p>
                              <p>Hemos recibido una solicitud de restablecimiento de contraseña de tu cuenta.</p>
                              <p>Haz clic en el botón que aparece a continuación para cambiar tu contraseña.</p>
                              <p>Ten en cuenta que este enlace es válido solo durante 24 horas. Una vez transcurrido el plazo, deberás volver a solicitar el restablecimiento de la contraseña.</p>
                              <p> {enlace}</p>"
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    await client.ConnectAsync(configuration["Smtp:Host"], int.Parse(configuration["Smtp:Port"]), MailKit.Security.SecureSocketOptions.Auto);
                    await client.AuthenticateAsync(configuration["Smtp:User"], configuration["Smtp:Pass"]);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    Console.WriteLine("Correo electrónico enviado");
                }

                return Ok("Resetcorrecto");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }


        private string GeneratePasswordResetToken(Propietario propietario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, propietario.Email),
                    new Claim(ClaimTypes.NameIdentifier, propietario.IdPropietario.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        //cambiar contraseña android 
        // PUT api/<controller>/changePassword
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromForm] string currentPassword, [FromForm] string newPassword)
        {
            try
            {
                var usuario = User.Identity.Name;
                var propietario = await contexto.propietarios.SingleOrDefaultAsync(x => x.Email == usuario);
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado.");
                }

                // Verificar la contraseña actual
                string hashedCurrentPassword = HashPassword(currentPassword);
                if (propietario.Clave != hashedCurrentPassword)
                {
                    return BadRequest("La contraseña actual es incorrecta.");
                }

                // Cambiar la contraseña
                propietario.Clave = HashPassword(newPassword);
                contexto.propietarios.Update(propietario);
                await contexto.SaveChangesAsync();

                return Ok(new { message = "La contraseña ha sido cambiada exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // GET api/<controller>/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await contexto.propietarios.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<controller>/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginView loginView)
        {
            try
            {
                loginView.Clave = loginView.Clave.TrimEnd('\n');
                string hashed = HashPassword(loginView.Clave);
                var p = await contexto.propietarios.FirstOrDefaultAsync(x => x.Email == loginView.Usuario);


                if (p == null || p.Clave != hashed)
                {
                    Console.WriteLine(p);
                    Console.WriteLine(loginView.Clave + " el hasheo seria del login: " + hashed);
                    return BadRequest("Nombre de usuario o clave incorrecta");
                }
                else
                {
                    Console.WriteLine("aca entro hasheada bien  " + hashed);
                    var key = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.Email),
                        new Claim("FullName", p.Nombre + " " + p.Apellido),
                        new Claim(ClaimTypes.Role, "Propietario"),
                        new Claim(ClaimTypes.NameIdentifier, p.IdPropietario.ToString())
                    };


                    var token = new JwtSecurityToken(
                        issuer: configuration["TokenAuthentication:Issuer"],
                        audience: configuration["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Propietario entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await contexto.propietarios.AddAsync(entidad);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = entidad.IdPropietario }, entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // PUT api/<controller>/actualizar
        [HttpPut("actualizar")]
        public async Task<IActionResult> Put([FromBody] Propietario entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var usuarioActual = User.Identity.Name; // Obtener el usuario actualmente autenticado
                    var original = await contexto.propietarios.FirstOrDefaultAsync(x => x.Email == usuarioActual);

                    if (original == null)
                    {
                        return NotFound("Propietario no encontrado");
                    }

                    // Actualizar solo los campos modificados
                    if (!String.IsNullOrEmpty(entidad.Clave))
                    {
                        original.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: entidad.Clave,
                            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));
                    }

                    original.Nombre = entidad.Nombre ?? original.Nombre;
                    original.Apellido = entidad.Apellido ?? original.Apellido;
                    original.Dni = entidad.Dni ?? original.Dni;
                    original.Email = entidad.Email ?? original.Email;
                    original.Telefono = entidad.Telefono ?? original.Telefono;

                    // Guardar los cambios en la entidad rastreada original
                    await contexto.SaveChangesAsync();

                    // Verificar los valores actualizados
                    Console.WriteLine($"Nombre: {original.Nombre}");
                    Console.WriteLine($"Apellido: {original.Apellido}");
                    Console.WriteLine($"Dni: {original.Dni}");
                    Console.WriteLine($"Telefono: {original.Telefono}");
                    Console.WriteLine($"Email: {original.Email}");

                    return Ok(original);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




        private string HashPassword(string password)
        {
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
        private string UnhashPassword(string hashedPassword)
        {
            byte[] hashedBytes = Convert.FromBase64String(hashedPassword);
            string password = System.Text.Encoding.UTF8.GetString(hashedBytes);

            return password;
        }

    }
}