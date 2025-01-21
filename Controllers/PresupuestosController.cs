using EspacioTp5;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rapositoriosTP5;
using tl2_tp6_2024_Days45.ViewModel;
namespace tl2_tp6_2024_Days45.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly ILogger<PresupuestosController> _logger;
        private readonly IPresupuestoRepository _repositorioPresupuestos;
        private readonly IUsuariosRepository _repositorioUsuarios; // Asumimos que tienes un repositorio de usuarios

        public PresupuestosController(ILogger<PresupuestosController> logger, IPresupuestoRepository repositorioPresupuestos, IUsuariosRepository repositorioUsuarios)
        {
            _logger = logger;
            _repositorioPresupuestos = repositorioPresupuestos;
            _repositorioUsuarios = repositorioUsuarios;
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
            return View(new Presupuestos(DateTime.Now, new Usuarios("", "", "", ""))); // Asegúrate de que los parámetros estén correctos
        }

        [HttpPost]
        public IActionResult Crear(DateTime fechaCreacion, int idUsuario)
        {
            try
            {
                // Obtener el usuario por su id
                var usuario = _repositorioPresupuestos.ObtenerUsuario(idUsuario);
                if (usuario == null)
                {
                    _logger.LogWarning("Usuario con ID {IdUsuario} no encontrado", idUsuario);
                    return View("Error");
                }

                var presupuesto = new Presupuestos(fechaCreacion, usuario);
                _repositorioPresupuestos.CrearPresupuesto(presupuesto);

                _logger.LogInformation("Presupuesto creado para el usuario con ID {IdUsuario}", idUsuario);
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
                // Obtener el presupuesto existente
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);

                if (presupuesto == null)
                {
                    _logger.LogWarning("No se encontró el presupuesto con ID {Id}", id);
                    return NotFound();
                }

                // Crear el ViewModel para la modificación
                var viewModel = new ModificarPresupuestoViewModel
                {
                    IdPresupuesto = presupuesto.IdPresupuesto,
                    FechaCreacion = presupuesto.FechaCreacion,
                    Detalles = (presupuesto.Detalle ?? new List<PresupuestosDetalle>())
                        .Where(d => d.Producto != null)
                        .Select(d => new DetalleModificacionViewModel
                        {
                            IdProducto = d.Producto?.IdProducto ?? 0,
                            NombreProducto = d.Producto?.Descripcion ?? string.Empty,
                            Cantidad = d.Cantidad,
                            Precio = (int)d.Producto?.Precio // Cambiado a int
                        }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el presupuesto para modificar");
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult Modificar(ModificarPresupuestoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // Registra los errores de validación
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    _logger.LogWarning("Error de validación: {ErrorMessage}", error.ErrorMessage);
                }
                return View(viewModel);
            }

            try
            {
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(viewModel.IdPresupuesto);

                if (presupuesto == null)
                {
                    _logger.LogWarning("No se encontró el presupuesto con ID {Id}", viewModel.IdPresupuesto);
                    return NotFound();
                }

                // Actualizar la fecha y los detalles en el repositorio
                presupuesto.ModificarFecha(viewModel.FechaCreacion); // Actualiza la fecha en el objeto
                _repositorioPresupuestos.ModificarPresupuesto(presupuesto, viewModel.Detalles); // Llama al repositorio para actualizar

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar el presupuesto: {Message}", ex.Message);
                var errorViewModel = new ErrorViewModel
                {
                    RequestId = HttpContext.TraceIdentifier
                };
                return View("Error", errorViewModel);
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
                ViewBag.Productos = new ProductoRepository().ListarProductos();
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
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
                if (presupuesto == null)
                {
                    return NotFound();
                }

                // Restricción para clientes: solo pueden ver su propio presupuesto
                var rol = HttpContext.Session.GetString("UserRole");
                var userId = HttpContext.Session.GetInt32("UserId");

                if (rol == "Cliente" && presupuesto.Usuario.IdUsuario != userId)
                {
                    return Forbid(); // Prohibido ver otros presupuestos
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
