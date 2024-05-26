namespace Inmobiliaria.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

public class RepositorioInmueble
{

    public RepositorioInmueble()
    {

    }
    readonly String ConnectionString = "Server=localhost;Database=Inmobiliaria;User=root;Password=";


    public IList<Inmueble> GetInmuebles()
    {
        var inmuebles = new List<Inmueble>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $"SELECT {nameof(Inmueble.IdInmueble)}, {nameof(Inmueble.Direccion)}, {nameof(Inmueble.Uso)}, {nameof(Inmueble.Tipo)}, {nameof(Inmueble.Ambientes)}, {nameof(Inmueble.Superficie)}, {nameof(Inmueble.Valor)}, i.{nameof(Inmueble.IdPropietario)}, p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)} FROM inmuebles i INNER JOIN propietarios p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)};";

            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Uso = reader.GetString(nameof(Inmueble.Uso)),
                            Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
                            Valor = reader.GetDecimal(nameof(Inmueble.Valor)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido))
                            }
                        });

                    }

                    connection.Close();
                }
            }
        }
        return inmuebles;
    }

    public Inmueble? GetInmueble(int id)
    {
        Inmueble? inmueble = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $"SELECT {nameof(Inmueble.IdInmueble)}, {nameof(Inmueble.Direccion)}, {nameof(Inmueble.Uso)}, {nameof(Inmueble.Tipo)}, {nameof(Inmueble.Ambientes)}, {nameof(Inmueble.Superficie)}, {nameof(Inmueble.Valor)}, {nameof(Inmueble.Latitud)}, {nameof(Inmueble.Longitud)}, i.{nameof(Inmueble.IdPropietario)}, p.{nameof(Propietario.Nombre)}, p.{nameof(Propietario.Apellido)} FROM inmuebles i JOIN propietarios p ON i.{nameof(Inmueble.IdPropietario)} = p.{nameof(Propietario.IdPropietario)} WHERE {nameof(Inmueble.IdInmueble)} = @IdInmueble;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdInmueble", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inmueble = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Uso = reader.GetString(nameof(Inmueble.Uso)),
                            Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                            Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                            Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
                            Valor = reader.GetDecimal(nameof(Inmueble.Valor)),
                            Latitud = reader.GetDecimal(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDecimal(nameof(Inmueble.Longitud)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
                            Duenio = new Propietario
                            {
                                IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
                                Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                Apellido = reader.GetString(nameof(Propietario.Apellido))
                            }
                        };
                    }
                }
            }
        }
        return inmueble;
    }

    public int CrearInmueble(Inmueble inmueble)
    {
        int Id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"INSERT INTO inmuebles (
                {nameof(Inmueble.Direccion)},{nameof(Inmueble.Uso)},{nameof(Inmueble.Tipo)},{nameof(Inmueble.Ambientes)},{nameof(Inmueble.Superficie)},{nameof(Inmueble.Latitud)},{nameof(Inmueble.Longitud)},{nameof(Inmueble.Valor)},{nameof(Inmueble.IdPropietario)}
            ) 
                VALUES (@{nameof(Inmueble.Direccion)}, @{nameof(Inmueble.Uso)},@{nameof(Inmueble.Tipo)},@{nameof(Inmueble.Ambientes)},@{nameof(Inmueble.Superficie)},@{nameof(Inmueble.Latitud)},@{nameof(Inmueble.Longitud)},@{nameof(Inmueble.Valor)},@{nameof(Inmueble.IdPropietario)}
                );
                SELECT LAST_INSERT_ID();";


            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", inmueble.IdInmueble);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Direccion)}", inmueble.Direccion);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Uso)}", inmueble.Uso);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Tipo)}", inmueble.Tipo);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Ambientes)}", inmueble.Ambientes);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Superficie)}", inmueble.Superficie);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Valor)}", inmueble.Valor);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Latitud)}", inmueble.Latitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Longitud)}", inmueble.Longitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);

                connection.Open();
                Id = Convert.ToInt32(command.ExecuteScalar());
                inmueble.IdInmueble = Id;
                connection.Close();
            }

        }
        return Id;
    }



    public int ModificarInmueble(Inmueble inmueble)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE inmuebles 
                     SET {nameof(Inmueble.Direccion)} = @{nameof(Inmueble.Direccion)}, 
                     {nameof(Inmueble.Uso)} = @{nameof(Inmueble.Uso)},
                     {nameof(Inmueble.Tipo)} = @{nameof(Inmueble.Tipo)},
                     {nameof(Inmueble.Ambientes)} = @{nameof(Inmueble.Ambientes)},
                     {nameof(Inmueble.Superficie)} = @{nameof(Inmueble.Superficie)},
                     {nameof(Inmueble.Latitud)} = @{nameof(Inmueble.Latitud)},
                     {nameof(Inmueble.Longitud)} = @{nameof(Inmueble.Longitud)},
                     {nameof(Inmueble.Valor)} = @{nameof(Inmueble.Valor)},
                     {nameof(Inmueble.IdPropietario)} = @{nameof(Inmueble.IdPropietario)}
                     WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", inmueble.IdInmueble);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Direccion)}", inmueble.Direccion);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Uso)}", inmueble.Uso);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Tipo)}", inmueble.Tipo);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Ambientes)}", inmueble.Ambientes);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Superficie)}", inmueble.Superficie);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Valor)}", inmueble.Valor);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Latitud)}", inmueble.Latitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.Longitud)}", inmueble.Longitud);
                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdPropietario)}", inmueble.IdPropietario);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }
    [Authorize(Policy = "Administrador")]

    public int EliminarInmueble(int id)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"DELETE FROM inmuebles WHERE {nameof(Inmueble.IdInmueble)} = @{nameof(Inmueble.IdInmueble)}";
            using (var command = new MySqlCommand(sql, connection))
            {

                command.Parameters.AddWithValue($"@{nameof(Inmueble.IdInmueble)}", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }
}