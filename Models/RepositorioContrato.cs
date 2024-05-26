namespace Inmobiliaria.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

public class RepositorioContrato
{


    public RepositorioContrato() { }


    readonly String ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";
    public IList<Contrato> GetContratos()
    {
        var contratos = new List<Contrato>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"
            SELECT 
                c.{nameof(Contrato.IdContrato)}, 
                c.{nameof(Contrato.FechaInicio)}, 
                c.{nameof(Contrato.FechaFinalizacion)}, 
                c.{nameof(Contrato.MontoAlquiler)}, 
                c.{nameof(Contrato.Estado)}, 
                c.{nameof(Contrato.IdInquilino)}, 
                inq.{nameof(Inquilino.Nombre)}, 
                inq.{nameof(Inquilino.Apellido)}, 
                c.{nameof(Contrato.IdInmueble)},
                im.{nameof(Inmueble.Direccion)}
            FROM 
                contratos c 
            INNER JOIN 
                inquilinos inq ON c.{nameof(Contrato.IdInquilino)} = inq.{nameof(Inquilino.IdInquilino)}
            INNER JOIN
                inmuebles im ON c.{nameof(Contrato.IdInmueble)} = im.{nameof(Inmueble.IdInmueble)}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        contratos.Add(new Contrato
                        {
                            IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFinalizacion = reader.GetDateTime(nameof(Contrato.FechaFinalizacion)),
                            MontoAlquiler = reader.GetDecimal(nameof(Contrato.MontoAlquiler)),
                            Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                            IdInquilino = reader.GetInt32(nameof(Contrato.IdInquilino)),
                            Inquilino = new Inquilino
                            {
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido))
                            },
                            IdInmueble = reader.GetInt32(nameof(Contrato.IdInmueble)),
                            Inmueble = new Inmueble
                            {
                                Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                                Duenio = new Propietario
                                {
                                    Nombre = reader.GetString(nameof(Propietario.Nombre)),
                                    Apellido = reader.GetString(nameof(Propietario.Apellido))
                                }
                            }
                        });
                    }
                    connection.Close();
                }
            }
        }
        return contratos;
    }

    public Contrato? GetContrato(int id)
    {
        Contrato? contrato = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"
                SELECT 
                    c.{nameof(Contrato.IdContrato)}, 
                    c.{nameof(Contrato.FechaInicio)}, 
                    c.{nameof(Contrato.FechaFinalizacion)}, 
                    c.{nameof(Contrato.MontoAlquiler)}, 
                    c.{nameof(Contrato.Estado)}, 
                    c.{nameof(Contrato.IdInquilino)}, 
                    inq.{nameof(Inquilino.Nombre)}, 
                    inq.{nameof(Inquilino.Apellido)}, 
                    c.{nameof(Contrato.IdInmueble)},
                    im.{nameof(Inmueble.Direccion)}
                FROM 
                    contratos c 
                INNER JOIN 
                    inquilinos inq ON c.{nameof(Contrato.IdInquilino)} = inq.{nameof(Inquilino.IdInquilino)}
                INNER JOIN
                    inmuebles im ON c.{nameof(Contrato.IdInmueble)} = im.{nameof(Inmueble.IdInmueble)}
                WHERE 
                    c.{nameof(Contrato.IdContrato)} = @IdContrato";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdContrato", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        contrato = new Contrato
                        {
                            IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                            FechaInicio = reader.GetDateTime(nameof(Contrato.FechaInicio)),
                            FechaFinalizacion = reader.GetDateTime(nameof(Contrato.FechaFinalizacion)),
                            MontoAlquiler = reader.GetDecimal(nameof(Contrato.MontoAlquiler)),
                            Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                            IdInquilino = reader.GetInt32(nameof(Contrato.IdInquilino)),
                            IdInmueble = reader.GetInt32(nameof(Contrato.IdInmueble)),
                            Inquilino = new Inquilino
                            {
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido))
                            },
                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                                Direccion = reader.GetString(nameof(Inmueble.Direccion))
                            }
                        };
                    }
                }
            }
        }
        return contrato;
    }


    public int CrearContrato(Contrato contrato)
    {
        int Id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"
        SELECT COUNT(*) 
        FROM contratos 
        WHERE {nameof(Contrato.IdInmueble)} = @{nameof(Contrato.IdInmueble)} 
        AND (DATE({nameof(Contrato.FechaInicio)}) BETWEEN DATE(@{nameof(Contrato.FechaInicio)}) AND DATE(@{nameof(Contrato.FechaFinalizacion)}) 
            OR DATE({nameof(Contrato.FechaFinalizacion)}) BETWEEN DATE(@{nameof(Contrato.FechaInicio)}) AND DATE(@{nameof(Contrato.FechaFinalizacion)}))";


            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInmueble)}", contrato.IdInmueble);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaInicio)}", contrato.FechaInicio);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaFinalizacion)}", contrato.FechaFinalizacion);

                connection.Open();
                int cantidadContratosSuperpuestos = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();

                if (cantidadContratosSuperpuestos > 0)
                {
                    throw new Exception("El inmueble ya está ocupado en las fechas especificadas por otro contrato.");
                }
            }

            var insertQuery = $@"
    INSERT INTO contratos (
        {nameof(Contrato.FechaInicio)}, {nameof(Contrato.FechaFinalizacion)}, {nameof(Contrato.MontoAlquiler)}, {nameof(Contrato.Estado)}, {nameof(Contrato.IdInquilino)}, {nameof(Contrato.IdInmueble)}
    ) 
    VALUES (
        @{nameof(Contrato.FechaInicio)}, @{nameof(Contrato.FechaFinalizacion)}, @{nameof(Contrato.MontoAlquiler)}, @{nameof(Contrato.Estado)}, @{nameof(Contrato.IdInquilino)}, @{nameof(Contrato.IdInmueble)}
    );
    SELECT LAST_INSERT_ID();";

            using (var insertCommand = new MySqlCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue($"@{nameof(Contrato.FechaInicio)}", contrato.FechaInicio);
                insertCommand.Parameters.AddWithValue($"@{nameof(Contrato.FechaFinalizacion)}", contrato.FechaFinalizacion);
                insertCommand.Parameters.AddWithValue($"@{nameof(Contrato.MontoAlquiler)}", contrato.MontoAlquiler);
                insertCommand.Parameters.AddWithValue($"@{nameof(Contrato.Estado)}", contrato.Estado);
                insertCommand.Parameters.AddWithValue($"@{nameof(Contrato.IdInquilino)}", contrato.IdInquilino);
                insertCommand.Parameters.AddWithValue($"@{nameof(Contrato.IdInmueble)}", contrato.IdInmueble);
                connection.Open();
                Id = Convert.ToInt32(insertCommand.ExecuteScalar());
                connection.Close();
            }
            return Id;
        }
    }



    public int ModificarContrato(Contrato contrato)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $@"UPDATE contratos 
                     SET 
                         {nameof(Contrato.FechaInicio)} = @{nameof(Contrato.FechaInicio)}, 
                         {nameof(Contrato.FechaFinalizacion)} = @{nameof(Contrato.FechaFinalizacion)}, 
                         {nameof(Contrato.MontoAlquiler)} = @{nameof(Contrato.MontoAlquiler)}, 
                         {nameof(Contrato.Estado)} = @{nameof(Contrato.Estado)}, 
                         {nameof(Contrato.IdInquilino)} = @{nameof(Contrato.IdInquilino)}, 
                         {nameof(Contrato.IdInmueble)} = @{nameof(Contrato.IdInmueble)} 
                     WHERE 
                         {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", contrato.IdContrato);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaInicio)}", contrato.FechaInicio);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FechaFinalizacion)}", contrato.FechaFinalizacion);
                command.Parameters.AddWithValue($"@{nameof(Contrato.MontoAlquiler)}", contrato.MontoAlquiler);
                command.Parameters.AddWithValue($"@{nameof(Contrato.Estado)}", contrato.Estado);
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInquilino)}", contrato.IdInquilino);
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdInmueble)}", contrato.IdInmueble);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }
    [Authorize(Policy = "Administrador")]

    public int EliminarContrato(int id)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"DELETE FROM contratos WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        return 0;
    }

}
