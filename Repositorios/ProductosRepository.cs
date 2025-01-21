using System;
using System.Collections.Generic;
using EspacioTp5;
using Microsoft.Data.Sqlite;

namespace rapositoriosTP5
{
    public class ProductoRepository : IProductoRepository
    {
        private string cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";
        public void CrearProducto(Productos producto)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();

                var query =
                    "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Descripcion",
                        producto.Descripcion ?? string.Empty
                    );
                    command.Parameters.AddWithValue("@Precio", producto.Precio);
                    command.ExecuteNonQuery();
                }
            }
        }
        public void ModificarProducto(int id, Productos producto)
        {
            var query =
                "UPDATE productos SET descripcion = @descripcion, precio = @precio WHERE idProducto = @idProducto";
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idProducto", id);
                    command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                    command.Parameters.AddWithValue("@precio", producto.Precio);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public List<Productos> ListarProductos()
        {
            var productos = new List<Productos>();
            var query = "SELECT * FROM productos";
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new Productos(
                                Convert.ToInt32(reader["idProducto"]),
                                reader["descripcion"].ToString(),
                                Convert.ToInt32(reader["precio"])
                            );
                            productos.Add(producto);
                        }
                    }
                }
                connection.Close();
            }
            return productos;
        }

        public Productos ObtenerProducto(int id)
        {
            Productos producto = null;
            var query = "SELECT * FROM productos WHERE idProducto = @idProducto";
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idProducto", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            producto = new Productos(
                                Convert.ToInt32(reader["idProducto"]),
                                reader["descripcion"].ToString(),
                                Convert.ToInt32(reader["precio"])
                            );
                        }
                    }
                }
                connection.Close();
            }
            return producto;
        }

        public void EliminarProducto(int id)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var deleteDetalleQuery = @"DELETE FROM PresupuestosDetalle WHERE idProducto=$id";
                        using (var sqlCmd = new SqliteCommand(deleteDetalleQuery, connection, transaction))
                        {
                            sqlCmd.Parameters.AddWithValue("$id", id);
                            sqlCmd.ExecuteNonQuery();
                        }

                        var sqlQuery = @"DELETE FROM Productos WHERE idProducto=$id";
                        using (var sqlCmd = new SqliteCommand(sqlQuery, connection, transaction))
                        {
                            sqlCmd.Parameters.AddWithValue("$id", id);
                            sqlCmd.ExecuteNonQuery();
                        }

                        transaction.Commit(); 
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al eliminar producto: " + ex.Message);
                    }
                }
            }

        }

    }
}
