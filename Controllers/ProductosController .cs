using EspacioTp5;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rapositoriosTP5;

namespace tl2_tp6_2024_Days45.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ILogger<ProductosController> _logger;
        private readonly IProductoRepository _repositorioProductos;

        public ProductosController(ILogger<ProductosController> logger, IProductoRepository repositorioProductos)
        {
            _logger = logger;
            _repositorioProductos = repositorioProductos;
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

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Productos());
        }

        [HttpPost]
        public IActionResult Crear(string descripcion, int precio)
        {
            try
            {
                if (string.IsNullOrEmpty(descripcion) || precio < 0)
                {
                    ModelState.AddModelError("", "Descripción y precio son requeridos.");
                    return View();
                }
                
                var producto = new Productos(descripcion, precio);
                _repositorioProductos.CrearProducto(producto);
                _logger.LogInformation("Producto creado: {Descripcion} - Precio: {Precio}", descripcion, precio);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un producto con descripción {Descripcion}", descripcion);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                var producto = _repositorioProductos.ObtenerProducto(id);
                if (producto == null)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado", id);
                    return RedirectToAction("Index");
                }
                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {ProductoId} para su modificación", id);
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult Modificar(int idProducto, string descripcion, int precio)
        {
            try
            {
                if (string.IsNullOrEmpty(descripcion) || precio < 0)
                {
                    ModelState.AddModelError("", "Descripción y precio son requeridos.");
                    return View();
                }
                
                var producto = new Productos(idProducto, descripcion, precio);
                _repositorioProductos.ModificarProducto(idProducto, producto);
                _logger.LogInformation("Producto con ID {ProductoId} modificado", idProducto);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar el producto con ID {ProductoId}", idProducto);
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
                    _logger.LogWarning("Intento de eliminar un producto con ID {ProductoId} que no existe", id);
                    return RedirectToAction("Index");
                }
                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {ProductoId} para eliminación", id);
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult EliminarProducto(int id)
        {
            try
            {
                _repositorioProductos.EliminarProducto(id);
                _logger.LogInformation("Producto con ID {ProductoId} eliminado correctamente", id);

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
