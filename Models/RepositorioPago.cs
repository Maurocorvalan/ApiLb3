namespace Inmobiliaria.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using MySql.Data.MySqlClient;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;

    public class RepositorioPago
    {
        public RepositorioPago() { }

        readonly String ConnectionString = "Server=localhost;Database=inmobiliaria;User=root;Password=;";

        public IList<Pago> GetPagos()
        {
            var pagos = new List<Pago>();
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $"SELECT p.{nameof(Pago.IdPago)}, p.{nameof(Pago.FechaPago)}, p.{nameof(Pago.Monto)}, p.{nameof(Pago.Detalle)}, p.{nameof(Pago.Estado)}, p.{nameof(Pago.IdContrato)} FROM pagos p INNER JOIN contratos c ON p.{nameof(Pago.IdContrato)} = c.{nameof(Contrato.IdContrato)};";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagos.Add(new Pago()
                            {
                                IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                                Monto = reader.GetDecimal(nameof(Pago.Monto)),
                                FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                                Detalle = reader.GetString(nameof(Pago.Detalle)),
                                IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                                Estado = reader.GetBoolean(nameof(Pago.Estado)),
                                Contrato = new Contrato
                                {
                                    IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                                    Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                                }
                            });
                        }
                        connection.Close();
                    }
                }
            }
            return pagos;
        }


        public Pago? GetPago(int id)
        {
            Pago? pago = null;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"SELECT 
        p.{nameof(Pago.IdPago)}, 
        p.{nameof(Pago.FechaPago)}, 
        p.{nameof(Pago.Monto)}, 
        p.{nameof(Pago.Detalle)}, 
        p.{nameof(Pago.Estado)}, 
        p.{nameof(Pago.IdContrato)} 
        FROM 
            pagos p 
        INNER JOIN 
            contratos c ON p.{nameof(Pago.IdContrato)} = c.{nameof(Contrato.IdContrato)}
        WHERE 
            p.{nameof(Pago.IdPago)} = @IdPago";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdPago", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = new Pago()
                            {
                                IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                                Monto = reader.GetDecimal(nameof(Pago.Monto)),
                                FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                                Detalle = reader.GetString(nameof(Pago.Detalle)),
                                IdContrato = reader.GetInt32(nameof(Pago.IdContrato)),
                                Estado = reader.GetBoolean(nameof(Pago.Estado)),
                                Contrato = new Contrato
                                {
                                    IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                                    Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                                }
                            };
                        }
                    }
                }
            }
            return pago;
        }


        public int ModificarPago(Pago pago)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"UPDATE pagos 
                            SET 
                             {nameof(Pago.FechaPago)} = @{nameof(Pago.FechaPago)},
                             {nameof(Pago.Monto)} = @{nameof(Pago.Monto)},
                             {nameof(Pago.Detalle)} = @{nameof(Pago.Detalle)},
                             {nameof(Pago.Estado)} = @{nameof(Pago.Estado)},
                             {nameof(Pago.IdContrato)} = @{nameof(Pago.IdContrato)}
                            WHERE 
                            {nameof(Pago.IdPago)} = @{nameof(Pago.IdPago)};";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Pago.IdPago)}", pago.IdPago);
                    command.Parameters.AddWithValue($"@{nameof(Pago.FechaPago)}", pago.FechaPago);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Monto)}", pago.Monto);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Detalle)}", pago.Detalle);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Estado)}", pago.Estado);
                    command.Parameters.AddWithValue($"@{nameof(Pago.IdContrato)}", pago.IdContrato);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return 0;
        }

        public int CrearPago(Pago pago)
        {
            int Id = 0;
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"INSERT INTO pagos ({nameof(Pago.FechaPago)}, {nameof(Pago.Monto)}, {nameof(Pago.Detalle)}, {nameof(Pago.Estado)}, {nameof(Pago.IdContrato)}) 
                             VALUES (@{nameof(Pago.FechaPago)}, @{nameof(Pago.Monto)}, @{nameof(Pago.Detalle)}, @{nameof(Pago.Estado)}, @{nameof(Pago.IdContrato)});
                             SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Pago.FechaPago)}", pago.FechaPago);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Monto)}", pago.Monto);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Detalle)}", pago.Detalle);
                    command.Parameters.AddWithValue($"@{nameof(Pago.Estado)}", pago.Estado);
                    command.Parameters.AddWithValue($"@{nameof(Pago.IdContrato)}", pago.IdContrato);
                    connection.Open();
                    Id = Convert.ToInt32(command.ExecuteScalar());
                    pago.IdPago = Id;
                    connection.Close();
                }
            }
            return Id;
        }

        [Authorize(Policy = "Administrador")]

        public int EliminarPago(int id)
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                var sql = $@"DELETE FROM pagos WHERE {nameof(Pago.IdPago)} = @{nameof(Pago.IdPago)};";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Pago.IdPago)}", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return 0;
        }
    }
}
