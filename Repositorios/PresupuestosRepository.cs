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

                // Insertar el producto en el detalle del presupuesto
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

        public Presupuestos ObtenerPresupuesto(int id)
        {
            DateTime fechaCreacion = DateTime.MinValue; // Fecha por defecto
            Clientes cliente = null;
            List<PresupuestosDetalle> detalles = new List<PresupuestosDetalle>();

            using (var connection = new SqliteConnection(cadenaConexion))
            {
                connection.Open();
                string query =
                    @"SELECT P.idPresupuesto, C.idCliente, C.Nombre, C.Email, C.Telefono, P.FechaCreacion, 
                                PD.idProducto, PD.Cantidad, Pr.Descripcion, Pr.Precio
                         FROM Presupuestos P
                         INNER JOIN Clientes C ON P.idCliente = C.idCliente
                         INNER JOIN PresupuestosDetalle PD ON P.idPresupuesto = PD.idPresupuesto
                         INNER JOIN Productos Pr ON PD.idProducto = Pr.idProducto
                         WHERE P.idPresupuesto = @id";

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (cliente == null) // Solo asignar el cliente una vez
                            {
                                cliente = new Clientes(
                                    reader.GetInt32(1), // idCliente
                                    reader.GetString(2), // Nombre del cliente
                                    reader.GetString(3), // Email del cliente
                                    reader.GetString(4) // Teléfono del cliente
                                );
                                fechaCreacion = reader.GetDateTime(5); // Leer la fecha de creación
                            }

                            // Crear el detalle del presupuesto
                            var producto = new Productos(
                                reader.GetInt32(6), // idProducto
                                reader.GetString(8), // Descripción del producto
                                reader.GetInt32(9) // Precio del producto
                            );
                            int cantidad = reader.GetInt32(7); // Obtener la cantidad
                            detalles.Add(new PresupuestosDetalle(producto, cantidad)); // Crear el detalle
                        }
                    }
                }
            }

            if (cliente == null)
            {
                // Si no se encontró el cliente, devolver null o lanzar una excepción
                throw new Exception("Cliente no encontrado para el presupuesto");
            }

            // Crear y devolver el presupuesto con el cliente y los detalles
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
