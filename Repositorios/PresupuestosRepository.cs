using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using EspacioTp5;

namespace rapositoriosTP5
{
    public class PresupuestoRepository : IPresupuestoRepository
    {
        private string cadenaConexion = "Data Source=db/Tienda.db;Cache=Shared";

        public void CrearPresupuesto(Presupuestos presupuesto)
        {
            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string queryString = "INSERT INTO Presupuestos (FechaCreacion, idUsuario) VALUES (@Fecha, @idUsuario);";
                var command = new SqliteCommand(queryString, connection);
                command.Parameters.AddWithValue("@Fecha", presupuesto.FechaCreacion.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@idUsuario", presupuesto.Usuario.IdUsuario); // Pasamos el idUsuario
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void AgregarProductoAPresupuesto(int presupuestoId, Productos producto, int cantidad)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idPresupuesto", presupuestoId);
                    command.Parameters.AddWithValue("@idProducto", producto.IdProducto);
                    command.Parameters.AddWithValue("@Cantidad", cantidad);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void EliminarPresupuesto(int id)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query = "DELETE FROM Presupuestos WHERE idPresupuesto = @id";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Presupuestos ObtenerPresupuesto(int id)
        {
            ProductoRepository productoRepository = new ProductoRepository();
            Usuarios usuario = null;
            DateTime fechaCreacion = DateTime.MinValue;
            List<PresupuestosDetalle> detalles = new List<PresupuestosDetalle>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query = @"SELECT p.idPresupuesto, p.FechaCreacion, p.idUsuario, u.Nombre, u.Rol, 
                                 pd.idProducto, pd.Cantidad 
                                 FROM Presupuestos p
                                 INNER JOIN Usuarios u ON p.idUsuario = u.id_usuario
                                 INNER JOIN PresupuestosDetalle pd ON p.idPresupuesto = pd.idPresupuesto
                                 WHERE p.idPresupuesto = @id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (fechaCreacion == DateTime.MinValue) // Solo asignar una vez
                            {
                                fechaCreacion = reader.GetDateTime(1);
                                int idUsuario = reader.GetInt32(2);
                                string nombreUsuario = reader.GetString(3);
                                string usuarioNombre = reader.GetString(2); // Lo asumo como la columna de usuario
                                string rolUsuario = reader.GetString(4);

                                // Usamos el constructor adecuado de Usuarios con idUsuario, nombre, usuarioNombre, contraseña (vacía), rolUsuario
                                usuario = new Usuarios(idUsuario, nombreUsuario, usuarioNombre, "", rolUsuario);
                            }

                            var producto = productoRepository.ObtenerProducto(reader.GetInt32(5));
                            int cantidad = reader.GetInt32(6);
                            detalles.Add(new PresupuestosDetalle(producto, cantidad));
                        }
                    }
                }
            }

            return new Presupuestos(id, fechaCreacion, usuario, detalles); // Pasamos el objeto Usuario al constructor
        }

        public List<Presupuestos> ListarPresupuestos()
        {
            List<Presupuestos> listaPresupuestos = new List<Presupuestos>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query = "SELECT idPresupuesto, FechaCreacion, idUsuario FROM Presupuestos";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idPresupuesto = reader.GetInt32(0);
                            DateTime fechaCreacion = reader.GetDateTime(1);
                            int idUsuario = reader.GetInt32(2);

                            // Obtenemos el objeto Usuario usando su id
                            Usuarios usuario = ObtenerUsuario(idUsuario);

                            listaPresupuestos.Add(new Presupuestos(idPresupuesto, fechaCreacion, usuario));
                        }
                    }
                }
            }

            return listaPresupuestos;
        }

        public Usuarios ObtenerUsuario(int idUsuario)
        {
            // Lógica para obtener el objeto Usuario por idUsuario desde la base de datos
            string query = "SELECT id_usuario, Nombre, Usuario, Contraseña, Rol FROM Usuarios WHERE id_usuario = @idUsuario";
            Usuarios usuario = null;
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idUsuario", idUsuario);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Usamos el constructor de Usuarios con todos los parámetros necesarios
                            usuario = new Usuarios(
                                reader.GetInt32(0), // id_usuario
                                reader.GetString(1), // Nombre
                                reader.GetString(2), // Usuario
                                reader.GetString(3), // Contraseña
                                reader.GetString(4)  // Rol
                            );
                        }
                    }
                }
            }
            return usuario;
        }

        public void ModificarPresupuesto(Presupuestos presupuesto)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string querystring = "UPDATE Presupuestos SET FechaCreacion = @FechaCreacion, idUsuario = @idUsuario WHERE idPresupuesto = @idPresupuesto";

                using (var command = new SqliteCommand(querystring, connection))
                {
                    command.Parameters.AddWithValue("@FechaCreacion", presupuesto.FechaCreacion.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@idUsuario", presupuesto.Usuario.IdUsuario); // Pasamos idUsuario
                    command.Parameters.AddWithValue("@idPresupuesto", presupuesto.IdPresupuesto);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
