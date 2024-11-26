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
        private readonly ProductoRepository _repositorioProductos;

        public PresupuestosController(ILogger<PresupuestosController> logger)
        {
            _logger = logger;
            _repositorioPresupuestos = new PresupuestoRepository();
            _repositorioClientes = new ClienteRepository();
            _repositorioProductos = new ProductoRepository();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var clientes = _repositorioClientes.ListarClientes();
            var presupuestos = _repositorioPresupuestos.ListarPresupuestos(clientes);
            return View(presupuestos);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var clientes = _repositorioClientes.ListarClientes();
            ViewBag.Clientes = clientes;
            return View();
        }

        [HttpPost]
        public IActionResult Crear(int clienteId, DateTime fechaCreacion)
        {
            try
            {
                var cliente = _repositorioClientes.ObtenerCliente(clienteId);

                var presupuesto = new Presupuestos(
                    0,
                    cliente,
                    new List<PresupuestosDetalle>(),
                    fechaCreacion
                );

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
        public IActionResult Modificar(int id)
        {
            try
            {
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
                if (presupuesto == null)
                {
                    _logger.LogWarning("Presupuesto con ID {Id} no encontrado", id);
                    return View(
                        "Error",
                        new ErrorViewModel { ErrorMessage = "Presupuesto no encontrado" }
                    );
                }

                var clientes = _repositorioClientes.ListarClientes();

                // Generar manualmente el HTML de las opciones
                var opcionesClientes = "";
                foreach (var cliente in clientes)
                {
                    var seleccionado =
                        cliente.IdCliente == presupuesto.Cliente.IdCliente ? "selected" : "";
                    opcionesClientes +=
                        $"<option value='{cliente.IdCliente}' {seleccionado}>{cliente.Nombre}</option>";
                }

                ViewBag.OpcionesClientes = opcionesClientes;
                ViewBag.Presupuesto = presupuesto;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el presupuesto con ID {Id}", id);
                return View(
                    "Error",
                    new ErrorViewModel { ErrorMessage = "Error al obtener el presupuesto." }
                );
            }
        }

        [HttpPost]
        public IActionResult Modificar(int id, int clienteId, DateTime fechaCreacion)
        {
            try
            {
                var cliente = _repositorioClientes.ObtenerCliente(clienteId);
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con ID {IdCliente} no encontrado", clienteId);
                    return View("Error");
                }

                var presupuesto = new Presupuestos(
                    id,
                    cliente,
                    new List<PresupuestosDetalle>(),
                    fechaCreacion
                );

                _repositorioPresupuestos.ModificarPresupuesto(presupuesto);

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
        public IActionResult Eliminar(int id)
        {
            try
            {
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
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
        public IActionResult AgregarProducto(int id)
        {
            try
            {
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
                ViewBag.Productos = _repositorioProductos.ListarProductos(); // Lista de productos disponibles
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
                var producto = _repositorioProductos.ObtenerProducto(idProducto);
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
        public IActionResult VerDetalle(int id)
        {
            try
            {
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
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
