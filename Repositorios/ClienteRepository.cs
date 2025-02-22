using EspacioTp5;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace rapositoriosTP5
{
    public class ClienteRepository : IClienteRepository
    {
        private string cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";

        public void CrearCliente(Clientes cliente)
        {
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    string query = "INSERT INTO Clientes (Nombre, Email, Telefono) VALUES (@Nombre, @Email, @Telefono)";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                        command.Parameters.AddWithValue("@Email", cliente.Email);
                        command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Aquí podrías manejar el error, loguearlo o lanzar una nueva excepción.
                throw new Exception("Error al crear el cliente", ex);
            }
        }

        public void ModificarCliente(int id, Clientes cliente)
        {
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    string query = "UPDATE Clientes SET Nombre = @Nombre, Email = @Email, Telefono = @Telefono WHERE IdCliente = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                        command.Parameters.AddWithValue("@Email", cliente.Email);
                        command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el cliente", ex);
            }
        }

        public List<Clientes> ListarClientes()
        {
            List<Clientes> lista = new List<Clientes>();
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    string query = "SELECT IdCliente, Nombre, Email, Telefono FROM Clientes";
                    using (var command = new SqliteCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Clientes(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los clientes", ex);
            }

            return lista;
        }

        public Clientes ObtenerCliente(int id)
        {
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    string query = "SELECT IdCliente, Nombre, Email, Telefono FROM Clientes WHERE IdCliente = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Clientes(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el cliente", ex);
            }
            return null;
        }

        public void EliminarCliente(int id)
        {
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    string query = "DELETE FROM Clientes WHERE IdCliente = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el cliente", ex);
            }
        }
    }
}
