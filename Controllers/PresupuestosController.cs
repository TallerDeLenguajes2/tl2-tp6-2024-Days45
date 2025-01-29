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
        private readonly IUsuariosRepository _repositorioUsuarios;
        private readonly IProductoRepository _repositorioProductos;

        public PresupuestosController(ILogger<PresupuestosController> logger, IPresupuestoRepository repositorioPresupuestos, IUsuariosRepository repositorioUsuarios, IProductoRepository repositorioProductos)
        {
            _logger = logger;
            _repositorioPresupuestos = repositorioPresupuestos;
            _repositorioUsuarios = repositorioUsuarios;
            _repositorioProductos = repositorioProductos;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var presupuestos = _repositorioPresupuestos.ListarPresupuestos();
            return View(presupuestos);
        }

        // GET: Presupuestos/Crear
        public IActionResult Crear()
        {
            var viewModel = new CrearPresupuestoViewModel
            {
                Usuarios = _repositorioUsuarios.ListarUsuarios(),
                Productos = _repositorioProductos.ListarProductos(),
                FechaCreacion = DateTime.Now
            };

            return View(viewModel);
        }

        // POST: Presupuestos/Crear
        [HttpPost]
        public IActionResult Crear(CrearPresupuestoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Usuarios = _repositorioUsuarios.ListarUsuarios();
                viewModel.Productos = _repositorioProductos.ListarProductos();
                return View(viewModel);
            }

            var usuario = _repositorioUsuarios.ObtenerUsuarioPorId(viewModel.IdUsuario);
            if (usuario == null)
            {
                ModelState.AddModelError("Usuario", "El usuario seleccionado no es válido.");
                viewModel.Usuarios = _repositorioUsuarios.ListarUsuarios();
                viewModel.Productos = _repositorioProductos.ListarProductos();
                return View(viewModel);
            }

            var detalles = viewModel.ProductosSeleccionados
                .Where(p => p.Cantidad > 0)
                .Select(p => new PresupuestosDetalle(
                    _repositorioProductos.ObtenerProducto(p.IdProducto),
                    p.Cantidad
                ))
                .ToList();

            if (!detalles.Any())
            {
                ModelState.AddModelError("Productos", "Debe seleccionar al menos un producto con cantidad válida.");
                viewModel.Usuarios = _repositorioUsuarios.ListarUsuarios();
                viewModel.Productos = _repositorioProductos.ListarProductos();
                return View(viewModel);
            }

            var presupuesto = new Presupuestos(
                viewModel.FechaCreacion,
                usuario,
                detalles
            );

            _repositorioPresupuestos.CrearPresupuesto(presupuesto);

            return RedirectToAction("Index");
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
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier }); // Asegúrate de pasar un modelo de error
            }
        }

        [HttpPost]
        public IActionResult Eliminar(int id, string confirmacion)
        {
            try
            {
                _repositorioPresupuestos.EliminarPresupuesto(id);
                _logger.LogInformation("Presupuesto con ID {Id} eliminado", id);
                return RedirectToAction("Index"); // Redirige al índice después de eliminar
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el presupuesto con ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier }); // Muestra la vista de error
            }
        }

        [HttpGet]
        public IActionResult AgregarProducto(int id)
        {
            try
            {
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
                if (presupuesto == null)
                {
                    _logger.LogWarning("No se encontró el presupuesto con ID {Id}", id);
                    return NotFound();
                }

                // No necesitas inicializar el usuario aquí, ya que debería estar cargado
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
                var producto = _repositorioProductos.ObtenerProducto(idProducto);
                if (producto == null)
                {
                    _logger.LogWarning("No se encontró el producto con ID {Id}", idProducto);
                    return NotFound();
                }

                // Agregar el producto al presupuesto
                _repositorioPresupuestos.AgregarProductoAPresupuesto(idPresupuesto, producto, cantidad);
                _logger.LogInformation("Producto con ID {IdProducto} agregado al presupuesto con ID {IdPresupuesto}", idProducto, idPresupuesto);
                return RedirectToAction("VerDetalle", new { id = idPresupuesto }); // Redirige a los detalles del presupuesto
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
