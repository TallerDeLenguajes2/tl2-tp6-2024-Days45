using System;
using System.Collections.Generic;
using EspacioTp5;
using Microsoft.Data.Sqlite;

namespace rapositoriosTP5
{
    public class PresupuestoRepository : IPresupuestoRepository
    {
        private string cadenaConexion = "Data Source=db/Tienda.db;Cache=Shared";

        // Crear presupuesto
        public void CrearPresupuesto(Presupuestos presupuesto)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string queryString =
                    "INSERT INTO Presupuestos (IdCliente, FechaCreacion) VALUES (@IdCliente, @Fecha);";
                using (var command = new SqliteCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", presupuesto.Cliente.IdCliente);
                    command.Parameters.AddWithValue(
                        "@Fecha",
                        presupuesto.FechaCreacion.ToString("yyyy-MM-dd")
                    );
                    command.ExecuteNonQuery();
                }
            }
        }

        // Agregar un producto al presupuesto
        public void AgregarProductoAPresupuesto(int presupuestoId, Productos producto, int cantidad)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query =
                    "INSERT INTO PresupuestosDetalle (idPresupuesto, idProducto, Cantidad) VALUES (@idPresupuesto, @idProducto, @Cantidad)";
                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idPresupuesto", presupuestoId);
                    command.Parameters.AddWithValue("@idProducto", producto.IdProducto);
                    command.Parameters.AddWithValue("@Cantidad", cantidad);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Eliminar presupuesto
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

        // Obtener presupuesto por ID
        public Presupuestos ObtenerPresupuesto(int id, Clientes cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente), "Cliente no encontrado");
            }

            ProductoRepository productoRepository = new ProductoRepository();
            DateTime fechaCreacion = DateTime.MinValue;
            List<PresupuestosDetalle> detalles = new List<PresupuestosDetalle>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query =
                    @"SELECT P.idPresupuesto, P.FechaCreacion, PD.idProducto, PD.Cantidad 
              FROM Presupuestos P
              LEFT JOIN PresupuestosDetalle PD ON P.idPresupuesto = PD.idPresupuesto
              WHERE P.idPresupuesto = @id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        // Verificar si hay al menos una fila
                        if (reader.Read())
                        {
                            // Obtener la fecha de creación
                            fechaCreacion = reader.GetDateTime(1);

                            // Leer detalles del presupuesto
                            do
                            {
                                if (!reader.IsDBNull(2)) // Verificar si el producto no es nulo
                                {
                                    var producto = productoRepository.ObtenerProducto(
                                        reader.GetInt32(2)
                                    );
                                    int cantidad = reader.GetInt32(3);
                                    detalles.Add(new PresupuestosDetalle(producto, cantidad));
                                }
                            } while (reader.Read());
                        }
                        else
                        {
                            // Si no se encuentra el presupuesto, retornar null
                            return null;
                        }
                    }
                }
            }

            // Retornar el objeto Presupuestos
            return new Presupuestos(id, cliente, detalles, fechaCreacion);
        }

        // Listar todos los presupuestos
        public List<Presupuestos> ListarPresupuestos(List<Clientes> listaClientes)
        {
            List<Presupuestos> listaPresupuestos = new List<Presupuestos>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query = "SELECT idPresupuesto, IdCliente, FechaCreacion FROM Presupuestos";
                using (var command = new SqliteCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int idPresupuesto = reader.GetInt32(0);
                            int idCliente = reader.IsDBNull(1) ? 0 : reader.GetInt32(1); // Manejar NULL
                            DateTime fechaCreacion = reader.GetDateTime(2);

                            // Buscar el cliente en la lista proporcionada
                            Clientes cliente = listaClientes.FirstOrDefault(c =>
                                c.IdCliente == idCliente
                            );
                            if (cliente == null)
                            {
                                // Manejar el caso donde el cliente no se encuentra
                                throw new Exception($"Cliente con ID {idCliente} no encontrado.");
                            }

                            listaPresupuestos.Add(
                                new Presupuestos(
                                    idPresupuesto,
                                    cliente,
                                    new List<PresupuestosDetalle>(),
                                    fechaCreacion
                                )
                            );
                        }
                    }
                }
            }

            return listaPresupuestos;
        }

        // Modificar un presupuesto
        public void ModificarPresupuesto(Presupuestos presupuesto)
        {
            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string querystring =
                    "UPDATE Presupuestos SET IdCliente = @IdCliente, FechaCreacion = @FechaCreacion WHERE idPresupuesto = @idPresupuesto";

                using (var command = new SqliteCommand(querystring, connection))
                {
                    command.Parameters.AddWithValue("@IdCliente", presupuesto.Cliente.IdCliente);
                    command.Parameters.AddWithValue(
                        "@FechaCreacion",
                        presupuesto.FechaCreacion.ToString("yyyy-MM-dd")
                    );
                    command.Parameters.AddWithValue("@idPresupuesto", presupuesto.IdPresupuesto);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
