using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using EspacioTp5;
using tl2_tp6_2024_Days45.ViewModel;

namespace rapositoriosTP5
{
    public class PresupuestoRepository : IPresupuestoRepository
    {
        private readonly string cadenaConexion = "Data Source=DB/Tienda.db;Cache=Shared";
        private readonly ILogger<PresupuestoRepository> logger;
        public PresupuestoRepository(ILogger<PresupuestoRepository> logger)
        {
            this.logger = logger;
        }
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
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        logger.LogError(ex, "Error al crear el presupuesto");
                        throw;
                    }
                }
            }
        }
        public void AgregarProductoAPresupuesto(int presupuestoId, Productos producto, int cantidad)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al agregar el producto al presupuesto");
                throw;
            }
        }
        public void EliminarPresupuesto(int id)
        {
            try
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar el presupuesto con ID {Id}", id);
                throw new Exception("Error al eliminar el presupuesto. Consulte con el administrador.", ex);
            }
        }
        public Presupuestos ObtenerPresupuesto(int id)
        {
            ProductoRepository productoRepository = new ProductoRepository();
            Clientes cliente = null;
            DateTime fechaCreacion = DateTime.MinValue;
            List<PresupuestosDetalle> detalles = new List<PresupuestosDetalle>();

            try
            {
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener el presupuesto con ID {Id}", id);
                throw new Exception("Error al obtener el presupuesto. Consulte con el administrador.", ex);
            }
        }
        public List<Presupuestos> ListarPresupuestos()
        {
            List<Presupuestos> listaPresupuestos = new List<Presupuestos>();
            ClienteRepository clienteRepo = new ClienteRepository();
            try
            {
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

                                Clientes cliente = clienteRepo.ObtenerCliente(idCliente);
                                listaPresupuestos.Add(new Presupuestos(idPresupuesto, fechaCreacion, cliente));
                            }
                        }
                    }
                }
                return listaPresupuestos;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al listar los presupuestos");
                throw new Exception("Error al listar los presupuestos. Consulte con el administrador.", ex);
            }
        }
        public List<Presupuestos> ListarPresupuestosPorCliente(int idCliente)
        {
            try
            {
                List<Presupuestos> listaPresupuestos = new List<Presupuestos>();
                using (var connection = new SqliteConnection(cadenaConexion))
                {
                    connection.Open();
                    string query = "SELECT idPresupuesto FROM Presupuestos WHERE idCliente = @idCliente";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@idCliente", idCliente);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listaPresupuestos.Add(ObtenerPresupuesto(reader.GetInt32(0)));
                            }
                        }
                    }
                }
                return listaPresupuestos;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al listar presupuestos del cliente {IdCliente}", idCliente);
                throw new Exception("Error al obtener los presupuestos del cliente. Consulte con el administrador.", ex);
            }
        }
        public void ModificarPresupuesto(Presupuestos presupuesto, List<DetalleModificacionViewModel> detalles)
        {
            try
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
                        string queryDetalle = @"INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad)
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al modificar el presupuesto con ID {IdPresupuesto}", presupuesto.IdPresupuesto);
                throw new Exception("Error al modificar el presupuesto. Consulte con el administrador.", ex);
            }
        }

    }
}