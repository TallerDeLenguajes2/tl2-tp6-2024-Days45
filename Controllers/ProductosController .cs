using EspacioTp5;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rapositoriosTP5;

namespace tl2_tp6_2024_Days45.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ILogger<ProductosController> _logger;
        private readonly ProductoRepository _repositorioProductos;

        public ProductosController(ILogger<ProductosController> logger)
        {
            _logger = logger;
            _repositorioProductos = new ProductoRepository();
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var productos = _repositorioProductos.ListarProductos();
                _logger.LogInformation("Se han listado {ProductoCount} productos", productos.Count);
                return View(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los productos");
                return View("Error");
            }
        }
        //index y crear hechos: el resto falta probar
        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Productos());
        }

        [HttpPost]
        public IActionResult Crear(string descripcion, int precio)
        {
            var producto = new Productos(descripcion, precio);
            _repositorioProductos.CrearProducto(producto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Modificar(int id, Productos producto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorioProductos.ModificarProducto(id, producto);
                    _logger.LogInformation("Producto con ID {ProductoId} modificado", id);
                    return RedirectToAction("Index");
                }
                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar el producto con ID {ProductoId}", id);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var producto = _repositorioProductos.ObtenerProducto(id);
                if (producto == null)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado", id);
                    return NotFound();
                }
                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al obtener el producto con ID {ProductoId} para eliminar",
                    id
                );
                return View("Error");
            }
        }

        [HttpPost]
        [ActionName("Eliminar")]
        public IActionResult EliminarProducto(int id)
        {
            try
            {
                _repositorioProductos.EliminarProducto(id);
                _logger.LogInformation("Producto con ID {ProductoId} eliminado", id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con ID {ProductoId}", id);
                return View("Error");
            }
        }
    }
}
