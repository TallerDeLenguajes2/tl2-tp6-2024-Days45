using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EspacioTp5;
using rapositoriosTP5;

namespace tl2_tp6_2024_Days45.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ILogger<ProductosController> _logger;
        private readonly ProductoRepository _repositorioProductos;

        // Constructor con ILogger y el repositorio de productos
        public ProductosController(ILogger<ProductosController> logger)
        {
            _logger = logger;
            _repositorioProductos = new ProductoRepository();
        }

        // Acción para listar todos los productos
        
        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var productos = _repositorioProductos.ListarProductos();
                _logger.LogInformation("Se han listado {ProductoCount} productos", productos.Count);
                return View(productos); // Retorna la vista con la lista de productos
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los productos");
                return View("Error"); // Si ocurre un error, muestra una vista de error
            }
        }

        // Acción para mostrar el formulario de alta (crear producto)
        [HttpGet]
        public IActionResult Crear()
        {
            return View(new Productos());
        }

        // Acción para crear un nuevo producto (recibe un producto)
        [HttpPost]
        public IActionResult Crear(Productos producto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorioProductos.CrearProducto(producto);
                    _logger.LogInformation("Producto creado: {Descripcion}", producto.Descripcion);
                    return RedirectToAction("Index"); // Redirige al listado de productos
                }
                return View(producto); // Si el modelo no es válido, vuelve al formulario
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el producto: {Descripcion}", producto.Descripcion);
                return View("Error"); // Si ocurre un error, muestra una vista de error
            }
        }

        // Acción para mostrar el formulario de modificación de un producto
        [HttpGet]
        public IActionResult Modificar(int id)
        {
            try
            {
                var producto = _repositorioProductos.ObtenerProducto(id);
                if (producto == null)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado", id);
                    return NotFound(); // Si no se encuentra el producto, muestra un 404
                }
                return View(producto); // Muestra el formulario para editar el producto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {ProductoId} para modificar", id);
                return View("Error"); // Si ocurre un error, muestra una vista de error
            }
        }

        // Acción para modificar un producto existente
        [HttpPost]
        public IActionResult Modificar(int id, Productos producto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _repositorioProductos.ModificarProducto(id, producto);
                    _logger.LogInformation("Producto con ID {ProductoId} modificado", id);
                    return RedirectToAction("Index"); // Redirige al listado de productos
                }
                return View(producto); // Si el modelo no es válido, vuelve al formulario
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar el producto con ID {ProductoId}", id);
                return View("Error"); // Si ocurre un error, muestra una vista de error
            }
        }

        // Acción para mostrar el formulario de eliminación de un producto
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var producto = _repositorioProductos.ObtenerProducto(id);
                if (producto == null)
                {
                    _logger.LogWarning("Producto con ID {ProductoId} no encontrado", id);
                    return NotFound(); // Si no se encuentra el producto, muestra un 404
                }
                return View(producto); // Muestra el formulario para eliminar el producto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID {ProductoId} para eliminar", id);
                return View("Error"); // Si ocurre un error, muestra una vista de error
            }
        }

        // Acción para eliminar un producto
        [HttpPost]
        [ActionName("Eliminar")]
        public IActionResult EliminarProducto(int id)
        {
            try
            {
                _repositorioProductos.EliminarProducto(id);
                _logger.LogInformation("Producto con ID {ProductoId} eliminado", id);
                return RedirectToAction("Index"); // Redirige al listado de productos
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con ID {ProductoId}", id);
                return View("Error"); // Si ocurre un error, muestra una vista de error
            }
        }
    }
}
