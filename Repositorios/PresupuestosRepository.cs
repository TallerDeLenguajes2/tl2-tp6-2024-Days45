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
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insertar el presupuesto principal con fecha de creación
                        string queryPresupuesto = "INSERT INTO Presupuestos (idPresupuesto, NombreDestinatario, FechaCreacion) VALUES (@idPresupuesto, @nombreDestinatario, @fechaCreacion)";
                        using (SqliteCommand command = new SqliteCommand(queryPresupuesto, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@idPresupuesto", presupuesto.IdPresupuesto);
                            command.Parameters.AddWithValue("@nombreDestinatario", presupuesto.NombreDestinatario);
                            command.Parameters.AddWithValue("@fechaCreacion", presupuesto.FechaCreacion); // Fecha de creación del presupuesto
                            command.ExecuteNonQuery();
                        }

                        // Insertar los detalles del presupuesto
                        string queryDetalle = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad)";
                        using (SqliteCommand command = new SqliteCommand(queryDetalle, connection, transaction))
                        {
                            foreach (var detalle in presupuesto.Detalle)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@idPresupuesto", presupuesto.IdPresupuesto);
                                command.Parameters.AddWithValue("@idProducto", detalle.Producto.IdProducto); // ID del producto en el detalle
                                command.Parameters.AddWithValue("@Cantidad", detalle.Cantidad); // Cantidad del producto
                                command.ExecuteNonQuery();
                            }
                        }

                        // Confirmar la transacción
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // En caso de error, hacer rollback
                        transaction.Rollback();
                        throw new Exception($"Error al crear presupuesto: {ex.Message}");
                    }
                }
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

                            listaPresupuestos.Add(new Presupuestos(idPresupuesto, nombreDestinatario,fechaCreacion, detalles)); // Pasamos la fecha de creación
                        }
                    }
                }
            }

            return listaPresupuestos;
        }

    }
}