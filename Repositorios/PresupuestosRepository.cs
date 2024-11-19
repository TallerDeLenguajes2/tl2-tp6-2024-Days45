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
                string queryString = $"INSERT INTO Presupuestos (NombreDestinatario, FechaCreacion) VALUES (@Nombre, @Fecha);";
                var command = new SqliteCommand(queryString, connection);
                command.Parameters.AddWithValue("@Nombre", presupuesto.NombreDestinatario);
                command.Parameters.AddWithValue("@Fecha", presupuesto.FechaCreacion.ToString("yyyy-MM-dd"));
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
            string nombreDestinatario = "";
            DateTime fechaCreacion = DateTime.MinValue; // Fecha por defecto
            List<PresupuestosDetalle> detalles = new List<PresupuestosDetalle>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query = @"SELECT idPresupuesto, NombreDestinatario, FechaCreacion, idProducto, Cantidad 
                         FROM Presupuestos P
                         INNER JOIN PresupuestosDetalle PD USING(idPresupuesto)
                         WHERE P.idPresupuesto = @id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (fechaCreacion == DateTime.MinValue) // Solo asignar una vez
                            {
                                nombreDestinatario = reader.GetString(1);
                                fechaCreacion = reader.GetDateTime(2); // Leer la fecha de creación
                            }

                            var producto = productoRepository.ObtenerProducto(reader.GetInt32(3));
                            int cantidad = reader.GetInt32(4);
                            detalles.Add(new PresupuestosDetalle(producto, cantidad));
                        }
                    }
                }
            }

            return new Presupuestos(id, nombreDestinatario, fechaCreacion, detalles); // Pasamos la fecha de creación
        }


        public List<Presupuestos> ListarPresupuestos()
        {
            List<Presupuestos> listaPresupuestos = new List<Presupuestos>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query = "SELECT idPresupuesto, NombreDestinatario, FechaCreacion FROM Presupuestos";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idPresupuesto = reader.GetInt32(0);
                            string nombreDestinatario = reader.GetString(1);
                            DateTime fechaCreacion = reader.GetDateTime(2); // Obtener la fecha de creación
                            List<PresupuestosDetalle> detalles = new List<PresupuestosDetalle>();

                            string queryDetalles = @"SELECT idProducto, Descripcion, Precio, Cantidad FROM PresupuestosDetalle
                                             INNER JOIN Productos USING(idProducto)
                                             WHERE idPresupuesto = @idPresupuesto";
                            var commandDetalles = new SqliteCommand(queryDetalles, connection);
                            commandDetalles.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                            using (var readerDetalles = commandDetalles.ExecuteReader())
                            {
                                while (readerDetalles.Read())
                                {
                                    var producto = new Productos(readerDetalles.GetInt32(0), readerDetalles.GetString(1), readerDetalles.GetInt32(2));
                                    detalles.Add(new PresupuestosDetalle(producto, readerDetalles.GetInt32(3)));
                                }
                            }

                            listaPresupuestos.Add(new Presupuestos(idPresupuesto, nombreDestinatario, fechaCreacion, detalles)); // Pasamos la fecha de creación
                        }
                    }
                }
            }

            return listaPresupuestos;
        }
        public void ModificarPresupuesto(Presupuestos presupuesto)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string querystring = "UPDATE Presupuestos SET NombreDestinatario = @NombreDestinatario, FechaCreacion = @FechaCreacion WHERE idPresupuesto = @idPresupuesto";

                using (var command = new SqliteCommand(querystring, connection))
                {
                    command.Parameters.AddWithValue("@NombreDestinatario", presupuesto.NombreDestinatario);
                    command.Parameters.AddWithValue("@FechaCreacion", presupuesto.FechaCreacion.ToString("yyyy-MM-dd")); // Formato de fecha: yyyy-MM-dd
                    command.Parameters.AddWithValue("@idPresupuesto", presupuesto.IdPresupuesto);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}