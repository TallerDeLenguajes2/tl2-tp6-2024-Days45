using System;
using System.Collections.Generic;
using EspacioTp5;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rapositoriosTP5;

namespace tl2_tp6_2024_Days45.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly ILogger<PresupuestosController> _logger;
        private readonly PresupuestoRepository _repositorioPresupuestos;
        private readonly ClienteRepository _repositorioClientes;

        public PresupuestosController(ILogger<PresupuestosController> logger)
        {
            _logger = logger;
            _repositorioPresupuestos = new PresupuestoRepository();
            _repositorioClientes = new ClienteRepository();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var clientes = _repositorioClientes.ListarClientes();
            var presupuestos = _repositorioPresupuestos.ListarPresupuestos(clientes);
            return View(presupuestos);
        }

        // Método GET para mostrar el formulario de creación de presupuesto
        [HttpGet]
        public IActionResult Crear()
        {
            // Obtener la lista de clientes para mostrar en el formulario
            var clientes = _repositorioClientes.ListarClientes();
            ViewBag.Clientes = clientes; // Pasar la lista de clientes a la vista
            return View();
        }

        // Método POST para crear el presupuesto
        [HttpPost]
        public IActionResult Crear(int clienteId, DateTime fechaCreacion)
        {
            try
            {
                // Obtener el cliente por ID desde el repositorio de clientes
                var cliente = _repositorioClientes.ObtenerCliente(clienteId);

                // Crear el presupuesto con el cliente correspondiente
                var presupuesto = new Presupuestos(
                    0,
                    cliente,
                    new List<PresupuestosDetalle>(),
                    fechaCreacion
                );

                // Llamamos al repositorio para guardar el presupuesto
                _repositorioPresupuestos.CrearPresupuesto(presupuesto);

                _logger.LogInformation(
                    "Presupuesto creado para {NombreDestinatario}",
                    cliente.Nombre
                );
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el presupuesto");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Modificar(int id, int clienteId)
        {
            try
            {
                // Obtener cliente y presupuesto
                var cliente = _repositorioClientes.ObtenerCliente(clienteId);
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con ID {IdCliente} no encontrado", clienteId);
                    return View(
                        "Error",
                        new ErrorViewModel { ErrorMessage = "Cliente no encontrado" }
                    );
                }

                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id, cliente);
                if (presupuesto == null)
                {
                    _logger.LogWarning("Presupuesto con ID {Id} no encontrado", id);
                    return View(
                        "Error",
                        new ErrorViewModel { ErrorMessage = "Presupuesto no encontrado" }
                    );
                }

                // Obtener lista de todos los clientes para el dropdown
                var clientes = _repositorioClientes.ListarClientes();

                // Generar las opciones de cliente como HTML
                var opcionesClientes = "";
                foreach (var c in clientes)
                {
                    var selected = c.IdCliente == clienteId ? "selected" : "";
                    opcionesClientes +=
                        $"<option value='{c.IdCliente}' {selected}>{c.Nombre}</option>";
                }

                // Pasar datos a la vista
                ViewBag.OpcionesClientes = opcionesClientes;
                ViewBag.Presupuesto = presupuesto;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el presupuesto con ID {Id}", id);
                return View(
                    "Error",
                    new ErrorViewModel
                    {
                        ErrorMessage = $"Error al obtener el presupuesto: {ex.Message}"
                    }
                );
            }
        }

        [HttpPost]
        public IActionResult Modificar(int id, int clienteId, DateTime fechaCreacion)
        {
            try
            {
                // Obtener el cliente actualizado
                var cliente = _repositorioClientes.ObtenerCliente(clienteId);

                // Verificar si el cliente es nulo
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con ID {IdCliente} no encontrado", clienteId);
                    return View("Error");
                }

                // Crear el objeto Presupuesto actualizado
                var presupuesto = new Presupuestos(
                    id,
                    cliente,
                    new List<PresupuestosDetalle>(),
                    fechaCreacion
                );

                // Llamamos al repositorio para modificar el presupuesto
                _repositorioPresupuestos.ModificarPresupuesto(presupuesto);

                // Log y redirección
                _logger.LogInformation("Presupuesto con ID {Id} modificado", id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar el presupuesto con ID {Id}", id);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Eliminar(int id, int clienteId)
        {
            try
            {
                var cliente = _repositorioClientes.ObtenerCliente(clienteId);
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id, cliente);
                if (presupuesto == null)
                {
                    _logger.LogWarning(
                        "Intento de eliminar un presupuesto inexistente con ID {Id}",
                        id
                    );
                    return RedirectToAction("Index");
                }
                return View(presupuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al cargar el presupuesto con ID {Id} para eliminar",
                    id
                );
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult Eliminar(int id, string confirmacion)
        {
            try
            {
                _repositorioPresupuestos.EliminarPresupuesto(id);
                _logger.LogInformation("Presupuesto con ID {Id} eliminado", id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el presupuesto con ID {Id}", id);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult AgregarProducto(int id, int clienteId)
        {
            try
            {
                var cliente = _repositorioClientes.ObtenerCliente(clienteId);
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id, cliente);
                ViewBag.Productos = new ProductoRepository().ListarProductos(); // Lista de productos disponibles
                return View(presupuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al cargar productos para el presupuesto con ID {Id}",
                    id
                );
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
        {
            try
            {
                var producto = new ProductoRepository().ObtenerProducto(idProducto);
                _repositorioPresupuestos.AgregarProductoAPresupuesto(
                    idPresupuesto,
                    producto,
                    cantidad
                );
                _logger.LogInformation(
                    "Producto con ID {IdProducto} agregado al presupuesto con ID {IdPresupuesto}",
                    idProducto,
                    idPresupuesto
                );
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al agregar producto al presupuesto con ID {IdPresupuesto}",
                    idPresupuesto
                );
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult VerDetalle(int id, int clienteId)
        {
            try
            {
                var cliente = _repositorioClientes.ObtenerCliente(clienteId);
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id, cliente);
                if (presupuesto == null)
                {
                    return NotFound();
                }
                return View(presupuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el presupuesto con ID {Id}", id);
                return View("Error");
            }
        }
    }
}
