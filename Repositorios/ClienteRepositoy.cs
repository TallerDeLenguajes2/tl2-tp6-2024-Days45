using System;
using System.Collections.Generic;
using EspacioTp5;
using Microsoft.Data.Sqlite;

namespace rapositoriosTP5
{
    public class ClienteRepository : IClienteRepository
    {
        private string cadenaConexion = "Data Source=db/Tienda.db;Cache=Shared";

        // Constructor que recibe la cadena de conexión
    

        // Crear un cliente
        public void CrearCliente(Clientes cliente)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "INSERT INTO Clientes (Nombre, Email) VALUES (@Nombre, @Email)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    command.Parameters.AddWithValue("@Email", cliente.Email);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Clientes ObtenerCliente(int id)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query =
                    "SELECT IdCliente, Nombre, Email, Telefono FROM Clientes WHERE IdCliente = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Clientes(
                                reader.GetInt32(reader.GetOrdinal("IdCliente")), // int
                                reader.GetString(reader.GetOrdinal("Nombre")), // string
                                reader.GetString(reader.GetOrdinal("Email")), // string
                                reader.GetString(reader.GetOrdinal("Telefono")) // string
                            );
                        }
                    }
                }
            }

            throw new Exception("Cliente no encontrado");
        }

        // Modificar un cliente por ID
        public void ModificarCliente(int id, Clientes cliente)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query =
                    "UPDATE Clientes SET Nombre = @Nombre, Email = @Email WHERE IdCliente = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    command.Parameters.AddWithValue("@Email", cliente.Email);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Eliminar un cliente por ID
        public void EliminarCliente(int id)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "DELETE FROM Clientes WHERE IdCliente = @Id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Clientes> ListarClientes()
        {
            var clientes = new List<Clientes>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                var query = "SELECT IdCliente, Nombre, Email, Telefono FROM Clientes"; // Incluye el campo Telefono
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clientes.Add(
                                new Clientes(
                                    reader.GetInt32(reader.GetOrdinal("IdCliente")), // int
                                    reader.GetString(reader.GetOrdinal("Nombre")), // string
                                    reader.GetString(reader.GetOrdinal("Email")), // string
                                    reader.GetString(reader.GetOrdinal("Telefono")) // string
                                )
                            );
                        }
                    }
                }
            }

            return clientes;
        }
    }
}
