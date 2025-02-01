using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using EspacioTp5;
using tl2_tp6_2024_Days45.ViewModel;
namespace rapositoriosTP5
{
    public class PresupuestoRepository : IPresupuestoRepository
    {
        private string cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";

        public void CrearPresupuesto(Presupuestos presupuesto)
        {
            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Crear presupuesto y obtener el ID generado
                        string queryPresupuesto = "INSERT INTO Presupuestos (FechaCreacion, idCliente) VALUES (@Fecha, @IdCliente); SELECT last_insert_rowid();";
                        var command = new SqliteCommand(queryPresupuesto, connection, transaction);
                        command.Parameters.AddWithValue("@Fecha", presupuesto.FechaCreacion.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@IdCliente", presupuesto.Cliente.IdCliente);

                        long idPresupuesto = (long)command.ExecuteScalar();

                        // Insertar detalles del presupuesto
                        foreach (var detalle in presupuesto.Detalle)
                        {
                            string queryDetalle = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad);";
                            var commandDetalle = new SqliteCommand(queryDetalle, connection, transaction);
                            commandDetalle.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                            commandDetalle.Parameters.AddWithValue("@idProducto", detalle.Producto.IdProducto);
                            commandDetalle.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                            commandDetalle.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void AgregarProductoAPresupuesto(int presupuestoId, Productos producto, int cantidad)
        {
            using (SqliteConnection connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string queryString = "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) " +
                                     "VALUES (@idPresupuesto, @idProducto, @Cantidad);";
                var command = new SqliteCommand(queryString, connection);
                command.Parameters.AddWithValue("@idPresupuesto", presupuestoId);
                command.Parameters.AddWithValue("@idProducto", producto.IdProducto);
                command.Parameters.AddWithValue("@Cantidad", cantidad);
                command.ExecuteNonQuery();
            }
        }

        public void EliminarPresupuesto(int id)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();

                // Primero, eliminar los detalles asociados
                string deleteDetailsQuery = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @id";
                using (var command = new SqliteCommand(deleteDetailsQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }

                // Luego, eliminar el presupuesto
                string deletePresupuestoQuery = "DELETE FROM Presupuestos WHERE idPresupuesto = @id";
                using (var command = new SqliteCommand(deletePresupuestoQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Presupuestos ObtenerPresupuesto(int id)
        {
            ProductoRepository productoRepository = new ProductoRepository();
            Clientes cliente = null;
            DateTime fechaCreacion = DateTime.MinValue;
            List<PresupuestosDetalle> detalles = new List<PresupuestosDetalle>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query = @"SELECT p.idPresupuesto, p.FechaCreacion, p.IdCliente, c.Nombre, c.Email, c.Telefono, 
                     pd.idProducto, pd.Cantidad 
                     FROM Presupuestos p
                     INNER JOIN Clientes c ON p.IdCliente = c.IdCliente
                     LEFT JOIN PresupuestosDetalle pd ON p.idPresupuesto = pd.idPresupuesto
                     WHERE p.idPresupuesto = @id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (fechaCreacion == DateTime.MinValue)
                            {
                                fechaCreacion = reader.GetDateTime(1);
                                int idCliente = reader.GetInt32(2);
                                string nombreCliente = reader.GetString(3);
                                string emailCliente = reader.GetString(4);
                                string telefonoCliente = reader.GetString(5);
                                cliente = new Clientes(idCliente, nombreCliente, emailCliente, telefonoCliente);
                            }

                            if (!reader.IsDBNull(6))
                            {
                                var producto = productoRepository.ObtenerProducto(reader.GetInt32(6));
                                int cantidad = reader.GetInt32(7);
                                detalles.Add(new PresupuestosDetalle(producto, cantidad));
                            }
                        }
                    }
                }
            }

            return new Presupuestos(id, fechaCreacion, cliente, detalles);
        }

        public List<Presupuestos> ListarPresupuestos()
        {
            List<Presupuestos> listaPresupuestos = new List<Presupuestos>();
            ClienteRepository clienteRepo = new ClienteRepository();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query = "SELECT idPresupuesto, FechaCreacion, IdCliente FROM Presupuestos";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idPresupuesto = reader.GetInt32(0);
                            DateTime fechaCreacion = reader.GetDateTime(1);
                            int idCliente = reader.GetInt32(2);

                            // Usar clienteRepo para obtener el cliente
                            Clientes cliente = clienteRepo.ObtenerCliente(idCliente);

                            listaPresupuestos.Add(new Presupuestos(idPresupuesto, fechaCreacion, cliente));
                        }
                    }
                }
            }

            return listaPresupuestos;
        }

        public void ModificarPresupuesto(Presupuestos presupuesto, List<DetalleModificacionViewModel> detalles)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();

                string queryPresupuesto = "UPDATE Presupuestos SET FechaCreacion = @FechaCreacion, IdCliente = @IdCliente WHERE idPresupuesto = @idPresupuesto";
                using (var command = new SqliteCommand(queryPresupuesto, connection))
                {
                    command.Parameters.AddWithValue("@FechaCreacion", presupuesto.FechaCreacion.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@IdCliente", presupuesto.Cliente.IdCliente);
                    command.Parameters.AddWithValue("@idPresupuesto", presupuesto.IdPresupuesto);
                    command.ExecuteNonQuery();
                }

                foreach (var detalleVM in detalles)
                {
                    string queryDetalle = @"
        INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad)
        VALUES (@idPresupuesto, @idProducto, @Cantidad)
        ON CONFLICT (idPresupuesto, idProducto) DO UPDATE
        SET Cantidad = @Cantidad";

                    using (var command = new SqliteCommand(queryDetalle, connection))
                    {
                        command.Parameters.AddWithValue("@idPresupuesto", presupuesto.IdPresupuesto);
                        command.Parameters.AddWithValue("@idProducto", detalleVM.IdProducto);
                        command.Parameters.AddWithValue("@Cantidad", detalleVM.Cantidad);
                        command.ExecuteNonQuery();
                    }

                    if (detalleVM.Cantidad <= 0)
                    {
                        string queryEliminar = "DELETE FROM PresupuestosDetalle WHERE idPresupuesto = @idPresupuesto AND idProducto = @idProducto";
                        using (var command = new SqliteCommand(queryEliminar, connection))
                        {
                            command.Parameters.AddWithValue("@idPresupuesto", presupuesto.IdPresupuesto);
                            command.Parameters.AddWithValue("@idProducto", detalleVM.IdProducto);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }

        }
    }
}
