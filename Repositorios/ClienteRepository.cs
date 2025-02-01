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

        public void ModificarCliente(int id, Clientes cliente)
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

        public List<Clientes> ListarClientes()
        {
            List<Clientes> lista = new List<Clientes>();
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
            return lista;
        }

        public Clientes ObtenerCliente(int id)
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
            return null;
        }

        public void EliminarCliente(int id)
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
    }
}
