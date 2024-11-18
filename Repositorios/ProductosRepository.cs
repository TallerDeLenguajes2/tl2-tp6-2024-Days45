using System;
using System.Collections.Generic;
using EspacioTp5;
using Microsoft.Data.Sqlite;

namespace rapositoriosTP5
{
    public class ProductoRepository : IProductoRepository
    {
        private string cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";

        // Crear un nuevo Producto (recibe un objeto Producto)
        /*CREATE TABLE Productos ( como esta formada la tabla productos en la base de datos
    idProducto  INTEGER PRIMARY KEY AUTOINCREMENT,
    Descripcion TEXT    NOT NULL,
    Precio      INTEGER NOT NULL);*/
        public void CrearProducto(Productos producto)
        {
            using (var connection = new SqliteConnection("Data Source=DB/Tienda.db"))
            {
                connection.Open();

                var query =
                    "INSERT INTO Productos (Descripcion, Precio) VALUES (@Descripcion, @Precio)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@Descripcion",
                        producto.Descripcion ?? string.Empty
                    ); // Asegura que Descripcion no sea null
                    command.Parameters.AddWithValue("@Precio", producto.Precio);

                    // Ejecuto la inserción
                    command.ExecuteNonQuery();
                }
            }
        }

        // Modificar un Producto existente (recibe un Id y un objeto Producto)
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

        // Listar todos los Productos registrados (devuelve un List de Producto)
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

        // Obtener detalles de un Producto por su ID (recibe un Id y devuelve un Producto)
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

        // Eliminar un Producto por ID (recibe un Id)
        public void EliminarProducto(int id)
        {
            var query = "DELETE FROM productos WHERE idProducto = @idProducto";
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idProducto", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}
