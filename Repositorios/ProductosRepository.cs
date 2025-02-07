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
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    var query = "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? string.Empty);
                        command.Parameters.AddWithValue("@Precio", producto.Precio);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el producto", ex);
            }
        }

        public void ModificarProducto(int id, Productos producto)
        {
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    var query = "UPDATE productos SET descripcion = @descripcion, precio = @precio WHERE idProducto = @idProducto";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idProducto", id);
                        command.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                        command.Parameters.AddWithValue("@precio", producto.Precio);
                        int filasAfectadas = command.ExecuteNonQuery();
                        if (filasAfectadas == 0)
                        {
                            throw new Exception("No se encontró el producto para modificar");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el producto", ex);
            }
        }

        public List<Productos> ListarProductos()
        {
            var productos = new List<Productos>();
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    var query = "SELECT * FROM productos";
                    using (var command = new SqliteCommand(query, connection))
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
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los productos", ex);
            }
            return productos;
        }

        public Productos ObtenerProducto(int id)
        {
            try
            {
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    var query = "SELECT idProducto, Descripcion, Precio FROM Productos WHERE idProducto = @id";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Productos(
                                    Convert.ToInt32(reader["idProducto"]),
                                    reader["Descripcion"].ToString(),
                                    Convert.ToInt32(reader["Precio"])
                                );
                            }
                        }
                    }
                }
                throw new Exception("Producto inexistente");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el producto", ex);
            }
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
                        var deleteDetalleQuery = "DELETE FROM PresupuestosDetalle WHERE idProducto=$id";
                        using (var sqlCmd = new SqliteCommand(deleteDetalleQuery, connection, transaction))
                        {
                            sqlCmd.Parameters.AddWithValue("$id", id);
                            sqlCmd.ExecuteNonQuery();
                        }

                        var sqlQuery = "DELETE FROM Productos WHERE idProducto=$id";
                        using (var sqlCmd = new SqliteCommand(sqlQuery, connection, transaction))
                        {
                            sqlCmd.Parameters.AddWithValue("$id", id);
                            int filasAfectadas = sqlCmd.ExecuteNonQuery();
                            if (filasAfectadas == 0)
                            {
                                throw new Exception("No se encontró el producto para eliminar");
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al eliminar producto", ex);
                    }
                }
            }
        }
    }
}
