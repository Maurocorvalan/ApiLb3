namespace Inmobiliaria.Models;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

public class RepositorioInquilino
{
    public RepositorioInquilino()
    {
    }
    readonly String ConnectionString = "Server=localhost;Database=Inmobiliaria;User=root;Password=";



    public IList<Inquilino> GetInquilinos()
    {
        var inquilinos = new List<Inquilino>();
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = $"SELECT {nameof(Inquilino.IdInquilino)}, {nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)}, {nameof(Inquilino.Dni)}, {nameof(Inquilino.Telefono)},{nameof(Inquilino.Email)}  FROM inquilinos";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inquilinos.Add(new Inquilino
                        {
                            IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Email = reader.GetString(nameof(Inquilino.Email)),
                        });

                    }
                    connection.Close();

                }
            }
        }
        return inquilinos;
    }
    public Inquilino? GetInquilino(int id)
    {
        Inquilino? inquilino = null;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"SELECT {nameof(Inquilino.IdInquilino)},{nameof(Inquilino.Nombre)},{nameof(Inquilino.Apellido)}, {nameof(Inquilino.Dni)}, {nameof(Inquilino.Telefono)},{nameof(Inquilino.Email)},
				{nameof(Inquilino.Nombre)}
			 FROM inquilinos
			 WHERE {nameof(Inquilino.IdInquilino)} = @{nameof(Inquilino.IdInquilino)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inquilino.IdInquilino)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
                            Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                            Apellido = reader.GetString(nameof(Inquilino.Apellido)),
                            Dni = reader.GetString(nameof(Inquilino.Dni)),
                            Telefono = reader.GetString(nameof(Inquilino.Telefono)),
                            Email = reader.GetString(nameof(Inquilino.Email)),
                        };
                    }
                }
            }
        }
        return inquilino;
    }
    public int CrearInquilino(Inquilino inquilino)
    {
        int Id = 0;
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"INSERT INTO inquilinos ({nameof(Inquilino.Nombre)}, {nameof(Inquilino.Apellido)}, {nameof(Inquilino.Dni)}, {nameof(Inquilino.Telefono)}, {nameof(Inquilino.Email)}) 
                     VALUES (@{nameof(Inquilino.Nombre)}, @{nameof(Inquilino.Apellido)}, @{nameof(Inquilino.Dni)}, @{nameof(Inquilino.Telefono)}, @{nameof(Inquilino.Email)});
                     SELECT LAST_INSERT_ID()";


            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Nombre)}", inquilino.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Apellido)}", inquilino.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Dni)}", inquilino.Dni);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Email)}", inquilino.Email);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Telefono)}", inquilino.Telefono);


                connection.Open();
                Id = Convert.ToInt32(command.ExecuteScalar());
                inquilino.IdInquilino = Id;
                connection.Close();
            }
        }
        return Id;
    }
    public int ModificarInquilino(Inquilino inquilino)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"UPDATE inquilinos 
                     SET {nameof(Inquilino.Nombre)} = @{nameof(Inquilino.Nombre)}, 
                         {nameof(Inquilino.Apellido)} = @{nameof(Inquilino.Apellido)}, 
                         {nameof(Inquilino.Dni)} = @{nameof(Inquilino.Dni)}, 
                         {nameof(Inquilino.Telefono)} = @{nameof(Inquilino.Telefono)}, 
                         {nameof(Inquilino.Email)} = @{nameof(Inquilino.Email)} 
                     WHERE {nameof(Inquilino.IdInquilino)} = @{nameof(Inquilino.IdInquilino)}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Inquilino.IdInquilino)}", inquilino.IdInquilino);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Nombre)}", inquilino.Nombre);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Apellido)}", inquilino.Apellido);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Dni)}", inquilino.Dni);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Email)}", inquilino.Email);
                command.Parameters.AddWithValue($"@{nameof(Inquilino.Telefono)}", inquilino.Telefono);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }


    [Authorize(Policy = "Administrador")]

    public int EliminarInquilino(int id)
    {
        using (var connection = new MySqlConnection(ConnectionString))
        {
            var sql = @$"DELETE FROM inquilinos WHERE {nameof(Inquilino.IdInquilino)} = @{nameof(Inquilino.IdInquilino)}";
            using (var command = new MySqlCommand(sql, connection))
            {

                command.Parameters.AddWithValue($"@{nameof(Inquilino.IdInquilino)}", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;
    }
}