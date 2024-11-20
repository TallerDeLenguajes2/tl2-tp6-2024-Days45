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

        public PresupuestosController(ILogger<PresupuestosController> logger)
        {
            _logger = logger;
            _repositorioPresupuestos = new PresupuestoRepository();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var presupuestos = _repositorioPresupuestos.ListarPresupuestos();
            return View(presupuestos);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            // Devolvemos el formulario para crear un presupuesto
            return View(new Presupuestos(0, string.Empty, DateTime.Now));
        }

        [HttpPost]
        public IActionResult Crear(string nombreDestinatario, DateTime fechaCreacion)
        {
            try
            {
                // Crear un nuevo objeto Presupuestos con los datos recibidos
                var presupuesto = new Presupuestos(nombreDestinatario, fechaCreacion);

                // Llamamos al repositorio para guardar el presupuesto
                _repositorioPresupuestos.CrearPresupuesto(presupuesto);

                _logger.LogInformation("Presupuesto creado para {NombreDestinatario}", presupuesto.NombreDestinatario);
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
                return View(presupuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el presupuesto con ID {Id}", id);
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult Modificar(int id, string nombreDestinatario, DateTime fechaCreacion)
        {
            try
            {
                // Crear el objeto Presupuesto con los nuevos datos
                var presupuesto = new Presupuestos(id, nombreDestinatario, fechaCreacion);

                // Llamamos a la función de repositorio para modificar el presupuesto
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
        public IActionResult Eliminar(int id)
        {
            try
            {
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
                if (presupuesto == null)
                {
                    _logger.LogWarning("Intento de eliminar un presupuesto inexistente con ID {Id}", id);
                    return RedirectToAction("Index");
                }
                return View(presupuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el presupuesto con ID {Id} para eliminar", id);
                return View("Error");
            }
        }

        // Método POST para realizar la eliminación
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
                ViewBag.Productos = new ProductoRepository().ListarProductos(); // Lista de productos disponibles
                return View(presupuesto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar productos para el presupuesto con ID {Id}", id);
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
        {
            try
            {
                var producto = new ProductoRepository().ObtenerProducto(idProducto);
                _repositorioPresupuestos.AgregarProductoAPresupuesto(idPresupuesto, producto, cantidad);
                _logger.LogInformation("Producto con ID {IdProducto} agregado al presupuesto con ID {IdPresupuesto}", idProducto, idPresupuesto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar producto al presupuesto con ID {IdPresupuesto}", idPresupuesto);
                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult VerDetalle(int id)
        {
            try
            {
                // Obtener el presupuesto usando el repositorio
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
