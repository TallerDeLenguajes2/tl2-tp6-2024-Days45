using System.Collections.Generic;
using EspacioTp5;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rapositoriosTP5;

namespace tl2_tp6_2024_Days45.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteRepository _repositorioClientes;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(
            IClienteRepository repositorioClientes,
            ILogger<ClientesController> logger
        )
        {
            _repositorioClientes = repositorioClientes;
            _logger = logger;
        }

        [HttpGet]
        // Index: Listar todos los clientes
        public IActionResult Index()
        {
            var clientes = _repositorioClientes.ListarClientes();
            return View(clientes);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(string nombre, string email, string telefono)
        {
            if (!ModelState.IsValid) // Verifica la validez del modelo
            {
                return View(); // Si hay errores, regresa a la vista con el estado actual
            }

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "El nombre y el email son obligatorios.");
                return View();
            }

            // Crear una nueva instancia del cliente usando el constructor sin ID
            var cliente = new Clientes(nombre, email, telefono);

            _repositorioClientes.CrearCliente(cliente);
            _logger.LogInformation("Cliente creado: {Nombre}", cliente.Nombre);
            return RedirectToAction("Index");
        }

        [HttpGet]
        // Modificar: GET
        public IActionResult Modificar(int id)
        {
            var cliente = _repositorioClientes.ObtenerCliente(id);
            if (cliente == null)
            {
                _logger.LogWarning("Cliente con ID {Id} no encontrado", id);
                return NotFound();
            }
            return View(cliente);
        }

        [HttpPost]
        public IActionResult Modificar(int idCliente, string nombre, string email, string telefono)
        {
            if (!ModelState.IsValid) // Verifica la validez del modelo
            {
                return View(); // Si hay errores, regresa a la vista con el estado actual
            }

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "El nombre y el email son obligatorios.");
                return View(); // Retorna la vista con el error
            }

            // Crear una nueva instancia de Clientes usando el constructor
            var clienteActualizar = new Clientes(idCliente, nombre, email, telefono);

            _repositorioClientes.ModificarCliente(idCliente, clienteActualizar);
            _logger.LogInformation("Cliente modificado: {Nombre}", nombre);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var cliente = _repositorioClientes.ObtenerCliente(id);
            if (cliente == null)
            {
                _logger.LogWarning("Cliente con ID {Id} no encontrado", id);
                return NotFound(); // Devuelve un error 404 si no se encuentra el cliente
            }
            return View(cliente); // Renderiza la vista de confirmación
        }

        [HttpPost]
        public IActionResult EliminarCliente(int idCliente)
        {
            var cliente = _repositorioClientes.ObtenerCliente(idCliente);
            if (cliente == null)
            {
                _logger.LogWarning("Cliente con ID {IdCliente} no encontrado", idCliente);
                return NotFound();
            }

            _repositorioClientes.EliminarCliente(idCliente);
            _logger.LogInformation("Cliente eliminado: {Nombre}", cliente.Nombre);
            return RedirectToAction("Index"); // Redirige a la lista después de eliminar
        }
    }
}
