namespace Inmobiliaria.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
public class RepositorioUsuario
{
    public RepositorioUsuario() { }

    readonly String ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";


    public Usuario GetUsuario(int id)
    {
        Usuario? usuario = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Usuario.IdUsuario)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, {nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.AvatarUrl)},{nameof(Usuario.Rol)} FROM usuarios WHERE {nameof(Usuario.IdUsuario)} = @{nameof(Usuario.IdUsuario)};";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.IdUsuario)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Clave = reader.GetString(nameof(Usuario.Clave)),
                            AvatarUrl = reader.IsDBNull(reader.GetOrdinal(nameof(Usuario.AvatarUrl))) ? null : reader.GetString(nameof(Usuario.AvatarUrl)),
                            Rol = reader.GetInt32(nameof(Usuario.Rol))

                        };
                    }
                }
            }
        }

        return usuario;
    }

    public int CrearUsuario(Usuario usuario)
    {
        int Id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"INSERT INTO usuarios ({nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, {nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.AvatarUrl)},{nameof(Usuario.Rol)})
                VALUES (@{nameof(Usuario.Nombre)}, @{nameof(Usuario.Apellido)}, @{nameof(Usuario.Email)}, @{nameof(Usuario.Clave)},@{nameof(Usuario.AvatarUrl)} ,@{nameof(Usuario.Rol)});
                SELECT LAST_INSERT_ID();";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.Nombre)}", usuario.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Apellido)}", usuario.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Email)}", usuario.Email);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Clave)}", usuario.Clave);
                if (String.IsNullOrEmpty(usuario.AvatarUrl))
                    command.Parameters.AddWithValue($"@{nameof(Usuario.AvatarUrl)}", DBNull.Value);
                else
                    command.Parameters.AddWithValue($"@{nameof(Usuario.AvatarUrl)}", usuario.AvatarUrl);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Rol)}", usuario.Rol);

                connection.Open();
                Id = Convert.ToInt32(command.ExecuteScalar());
                usuario.IdUsuario = Id;
                connection.Close();
            }
        }
        return Id;
    }
    public int ModificarUsuario(Usuario usuario)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE usuarios 
                     SET {nameof(Usuario.Nombre)} = @{nameof(Usuario.Nombre)}, 
                         {nameof(Usuario.Apellido)} = @{nameof(Usuario.Apellido)}, 
                         {nameof(Usuario.Email)} = @{nameof(Usuario.Email)}, 
                         {nameof(Usuario.Clave)} = @{nameof(Usuario.Clave)}, 
                         {nameof(Usuario.AvatarUrl)} = @{nameof(Usuario.AvatarUrl)}, 
                         {nameof(Usuario.Rol)} = @{nameof(Usuario.Rol)} 
                     WHERE {nameof(Usuario.IdUsuario)} = @{nameof(Usuario.IdUsuario)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.Nombre)}", usuario.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Apellido)}", usuario.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Email)}", usuario.Email);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Clave)}", usuario.Clave);
                command.Parameters.AddWithValue($"@{nameof(Usuario.AvatarUrl)}", usuario.AvatarUrl);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Rol)}", usuario.Rol);
                command.Parameters.AddWithValue($"@{nameof(Usuario.IdUsuario)}", usuario.IdUsuario);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }


    public IList<Usuario> GetUsuarios()
    {
        var usuarios = new List<Usuario>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Usuario.IdUsuario)}, {nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)}, {nameof(Usuario.Email)}, {nameof(Usuario.Clave)}, {nameof(Usuario.AvatarUrl)},{nameof(Usuario.Rol)} 
            FROM usuarios;";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Clave = reader.GetString(nameof(Usuario.Clave)),
                            AvatarUrl = reader.IsDBNull(reader.GetOrdinal(nameof(Usuario.AvatarUrl))) ? null : reader.GetString(nameof(Usuario.AvatarUrl)),
                            Rol = reader.GetInt32(nameof(Usuario.Rol))
                        });
                    }
                }
            }
        }
        return usuarios;
    }
    public Usuario? ObtenerPorEmail(string Email)
    {
        Usuario? usuario = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT 
            {nameof(Usuario.IdUsuario)}, 
            {nameof(Usuario.Nombre)},
            {nameof(Usuario.Apellido)},
            {nameof(Usuario.Email)},
            {nameof(Usuario.Clave)},
            {nameof(Usuario.AvatarUrl)},
            {nameof(Usuario.Rol)}
            FROM usuarios
            WHERE {nameof(Usuario.Email)} = @{nameof(Usuario.Email)};";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.Email)}", Email);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Clave = reader.GetString(nameof(Usuario.Clave)),
                            AvatarUrl = reader.IsDBNull(reader.GetOrdinal(nameof(Usuario.AvatarUrl))) ? null : reader.GetString(nameof(Usuario.AvatarUrl)),
                            Rol = reader.GetInt32(nameof(Usuario.Rol))
                        };
                    }
                }
                connection.Close();
            }
            return usuario;
        }

    }
    public Usuario ObtenerPorId(int id)
    {
        Usuario usuario = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $"SELECT * FROM usuarios WHERE {nameof(Usuario.IdUsuario)} = @{nameof(Usuario.IdUsuario)};";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.IdUsuario)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            IdUsuario = reader.GetInt32(reader.GetOrdinal(nameof(Usuario.IdUsuario))),
                            Nombre = reader.GetString(reader.GetOrdinal(nameof(Usuario.Nombre))),
                            Apellido = reader.GetString(reader.GetOrdinal(nameof(Usuario.Apellido))),
                            Email = reader.GetString(reader.GetOrdinal(nameof(Usuario.Email))),
                            Clave = reader.GetString(reader.GetOrdinal(nameof(Usuario.Clave))),
                            AvatarUrl = reader.IsDBNull(reader.GetOrdinal(nameof(Usuario.AvatarUrl))) ? null : reader.GetString(reader.GetOrdinal(nameof(Usuario.AvatarUrl)))
                        };
                    }
                }
            }
        }
        return usuario;
    }

    public int ModificarAvatar(Usuario usuario)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE usuarios SET {nameof(Usuario.AvatarUrl)} = @{nameof(Usuario.AvatarUrl)} WHERE {nameof(Usuario.IdUsuario)} = @{nameof(Usuario.IdUsuario)};";
            using (var command = new MySqlCommand(sql, connection))
            {
                if (String.IsNullOrEmpty(usuario.AvatarUrl))
                    command.Parameters.AddWithValue($"@{nameof(Usuario.AvatarUrl)}", DBNull.Value);
                else
                    command.Parameters.AddWithValue($"@{nameof(Usuario.AvatarUrl)}", usuario.AvatarUrl);
                command.Parameters.AddWithValue($"@{nameof(Usuario.IdUsuario)}", usuario.IdUsuario);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }

    public int ModificarClave(Usuario usuario)
    {
        int Id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE usuarios SET {nameof(Usuario.Clave)} = @{nameof(Usuario.Clave)} WHERE {nameof(Usuario.IdUsuario)} = @{nameof(Usuario.IdUsuario)};";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.Clave)}", usuario.Clave);
                command.Parameters.AddWithValue($"@{nameof(Usuario.IdUsuario)}", usuario.IdUsuario);
                connection.Open();
                command.ExecuteNonQuery();
                usuario.IdUsuario = Id;
                connection.Close();
            }
        }
        return Id;
    }
    public int ModificarDatos(Usuario usuario)
    {
        int Id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE usuarios SET {nameof(Usuario.Nombre)} = @{nameof(Usuario.Nombre)}, {nameof(Usuario.Apellido)} = @{nameof(Usuario.Apellido)}, {nameof(Usuario.Email)} = @{nameof(Usuario.Email)} WHERE {nameof(Usuario.IdUsuario)} = @{nameof(Usuario.IdUsuario)};";
            using (var command = new MySqlCommand(sql, connection))
            
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.Nombre)}", usuario.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Apellido)}", usuario.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Usuario.Email)}", usuario.Email);
                command.Parameters.AddWithValue($"@{nameof(Usuario.IdUsuario)}", usuario.IdUsuario);
                connection.Open();
                command.ExecuteNonQuery();
                usuario.IdUsuario = Id;
                connection.Close();
            }
        }
        return Id;
    }
    [Authorize(Policy = "Administrador")]

    public int EliminarUsuario(int id)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"DELETE FROM usuarios WHERE {nameof(Usuario.IdUsuario)} = @{nameof(Usuario.IdUsuario)};";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Usuario.IdUsuario)}", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }

}