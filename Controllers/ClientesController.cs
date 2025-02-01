using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rapositoriosTP5;
using EspacioTp5;
namespace tl2_tp6_2024_Days45.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ILogger<ClientesController> _logger;
        private readonly IClienteRepository _repositorioClientes;

        public ClientesController(ILogger<ClientesController> logger, IClienteRepository repositorioClientes)
        {
            _logger = logger;
            _repositorioClientes = repositorioClientes;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var clientes = _repositorioClientes.ListarClientes();
                _logger.LogInformation("Se han listado {ClienteCount} clientes", clientes.Count);
                return View(clientes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los clientes");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(string nombre, string email, string telefono)
        {
            var cliente = new Clientes(nombre, email, telefono);
            _repositorioClientes.CrearCliente(cliente);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                var cliente = _repositorioClientes.ObtenerCliente(id);
                if (cliente == null)
                {
                    _logger.LogWarning("Cliente con ID {ClienteId} no encontrado", id);
                    return RedirectToAction("Index");
                }
                return View(cliente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cliente con ID {ClienteId} para su modificación", id);
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult Modificar(int idCliente, string nombre, string email, string telefono)
        {
            try
            {
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(telefono))
                {
                    ModelState.AddModelError("", "Todos los campos son requeridos.");
                    return View();
                }
                var cliente = new Clientes(idCliente, nombre, email, telefono);
                _repositorioClientes.ModificarCliente(idCliente, cliente);
                _logger.LogInformation("Cliente con ID {ClienteId} modificado", idCliente);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar el cliente con ID {ClienteId}", idCliente);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            return View(_repositorioClientes.ObtenerCliente(id));
        }

        [HttpPost]
        public IActionResult EliminarCliente(int id)
        {
            _repositorioClientes.EliminarCliente(id);
            return RedirectToAction("Index");
        }
    }
}
