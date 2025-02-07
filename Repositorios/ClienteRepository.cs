using EspacioTp5;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace rapositoriosTP5
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string cadenaConexion;
        private readonly ILogger<ClienteRepository> _logger;

        public ClienteRepository(string cadenaConexion, ILogger<ClienteRepository> _logger)
        {
            this.cadenaConexion = cadenaConexion;
            this._logger = _logger;
        }

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

                        int filasAfectadas = command.ExecuteNonQuery();
                        if (filasAfectadas == 0)
                        {
                            throw new InvalidOperationException("No se pudo insertar el cliente en la base de datos.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el cliente {Cliente}", cliente.Nombre);
                throw new ApplicationException("Ocurrió un error al crear el cliente.", ex);
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

                        int filasAfectadas = command.ExecuteNonQuery();
                        if (filasAfectadas == 0)
                        {
                            throw new KeyNotFoundException($"No se encontró un cliente con Id {id} para modificar.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar el cliente con Id {Id}", id);
                throw new ApplicationException("Ocurrió un error al modificar el cliente.", ex);
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

                if (lista.Count == 0)
                {
                    throw new InvalidOperationException("No se encontraron clientes en la base de datos.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los clientes");
                throw new ApplicationException("Ocurrió un error al listar los clientes.", ex);
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

                throw new KeyNotFoundException($"No se encontró un cliente con Id {id}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cliente con Id {Id}", id);
                throw new ApplicationException("Ocurrió un error al obtener el cliente.", ex);
            }
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
                        int filasAfectadas = command.ExecuteNonQuery();
                        if (filasAfectadas == 0)
                        {
                            throw new KeyNotFoundException($"No se encontró un cliente con Id {id} para eliminar.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente con Id {Id}", id);
                throw new ApplicationException("Ocurrió un error al eliminar el cliente.", ex);
            }
        }
    }
}
