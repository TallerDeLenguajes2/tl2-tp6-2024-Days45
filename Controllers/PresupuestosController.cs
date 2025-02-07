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
        private readonly IClienteRepository _repositorioClientes;
        private readonly IProductoRepository _repositorioProductos;
        private readonly IUsuariosRepository _usuariosRepository;

        public PresupuestosController(ILogger<PresupuestosController> logger, IPresupuestoRepository repositorioPresupuestos, IClienteRepository repositorioClientes, IProductoRepository repositorioProductos, IUsuariosRepository usuariosRepository)
        {
            _logger = logger;
            _repositorioPresupuestos = repositorioPresupuestos;
            _repositorioClientes = repositorioClientes;
            _repositorioProductos = repositorioProductos;
            _usuariosRepository = usuariosRepository;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(rol))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Login");
            }

            if (rol == "Administrador")
            {
                var presupuestos = _repositorioPresupuestos.ListarPresupuestos();
                return View(presupuestos);
            }
            else if (rol == "Cliente")
            {
                var userId = HttpContext.Session.GetInt32("UserId");

                if (userId.HasValue)
                {
                    var idCliente = _usuariosRepository.ObtenerIdClientePorUsuario(userId.Value);

                    if (idCliente.HasValue)
                    {
                        var presupuestos = _repositorioPresupuestos.ListarPresupuestosPorCliente(idCliente.Value);
                        return View(presupuestos);
                    }
                }
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }


        [HttpGet]
        public IActionResult Crear()
        {
            var viewModel = new CrearPresupuestoViewModel
            {
                Clientes = _repositorioClientes.ListarClientes(),
                Productos = _repositorioProductos.ListarProductos(),
                FechaCreacion = DateTime.Now
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Crear(CrearPresupuestoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Clientes = _repositorioClientes.ListarClientes();
                viewModel.Productos = _repositorioProductos.ListarProductos();
                return View(viewModel);
            }

            var cliente = _repositorioClientes.ObtenerCliente(viewModel.IdCliente);
            if (cliente == null)
            {
                ModelState.AddModelError("Cliente", "El cliente seleccionado no es válido.");
                viewModel.Clientes = _repositorioClientes.ListarClientes();
                viewModel.Productos = _repositorioProductos.ListarProductos();
                return View(viewModel);
            }

            var detalles = viewModel.ProductosSeleccionados
                .Where(p => p.Cantidad > 0)
                .Select(p => new PresupuestosDetalle(
                    _repositorioProductos.ObtenerProducto(p.IdProducto),
                    p.Cantidad
                )).ToList();

            if (!detalles.Any())
            {
                ModelState.AddModelError("Productos", "Debe seleccionar al menos un producto con cantidad válida.");
                viewModel.Clientes = _repositorioClientes.ListarClientes();
                viewModel.Productos = _repositorioProductos.ListarProductos();
                return View(viewModel);
            }

            var presupuesto = new Presupuestos(viewModel.FechaCreacion, cliente, detalles);
            _repositorioPresupuestos.CrearPresupuesto(presupuesto);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Modificar(int id)
        {
            var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
            if (presupuesto == null) return NotFound();

            var viewModel = new ModificarPresupuestoViewModel
            {
                IdPresupuesto = presupuesto.IdPresupuesto,
                FechaCreacion = presupuesto.FechaCreacion,
                Detalles = presupuesto.Detalle.Select(d => new DetalleModificacionViewModel
                {
                    IdProducto = d.Producto.IdProducto,
                    NombreProducto = d.Producto.Descripcion,
                    Cantidad = d.Cantidad,
                    Precio = d.Producto.Precio
                }).ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Modificar(ModificarPresupuestoViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(viewModel.IdPresupuesto);
            if (presupuesto == null) return NotFound();

            presupuesto.ModificarFecha(viewModel.FechaCreacion);
            _repositorioPresupuestos.ModificarPresupuesto(presupuesto, viewModel.Detalles);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
            if (presupuesto == null) return RedirectToAction("Index");
            return View(presupuesto);
        }

        [HttpPost]
        public IActionResult Eliminar(int id, string confirmacion)
        {
            _repositorioPresupuestos.EliminarPresupuesto(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AgregarProducto(int id)
        {
            var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
            if (presupuesto == null) return NotFound();
            ViewBag.Productos = _repositorioProductos.ListarProductos();
            return View(presupuesto);
        }

        [HttpPost]
        public IActionResult AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
        {
            var producto = _repositorioProductos.ObtenerProducto(idProducto);
            if (producto == null) return NotFound();

            _repositorioPresupuestos.AgregarProductoAPresupuesto(idPresupuesto, producto, cantidad);
            return RedirectToAction("VerDetalle", new { id = idPresupuesto });
        }

        [HttpGet]
        public IActionResult VerDetalle(int id)
        {
            try
            {
                var rol = HttpContext.Session.GetString("UserRole");
                var userId = HttpContext.Session.GetInt32("UserId");
                var presupuesto = _repositorioPresupuestos.ObtenerPresupuesto(id);
                if (presupuesto == null)
                {
                    return NotFound(); 
                }

                if (rol == "Administrador")
                {
                    return View(presupuesto); 
                }
                else if (rol == "Cliente" && userId.HasValue)
                {
                    var idCliente = _usuariosRepository.ObtenerIdClientePorUsuario(userId.Value);
                    if (presupuesto.Cliente != null && presupuesto.Cliente.IdCliente == idCliente)
                    {
                        return View(presupuesto); 
                    }
                    else
                    {

                        return RedirectToAction("Error", "Home");
                    }
                }

                return RedirectToAction("Index", "Login"); 
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en VerDetalle: {ex.Message}", ex);
                return RedirectToAction("Error", "Home");
            }
        }



    }
}
