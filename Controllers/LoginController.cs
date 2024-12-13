using EspacioTp5;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rapositoriosTP5;
using tl2_tp6_2024_Days45.ViewModel;
namespace tl2_tp6_2024_Days45.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuariosRepository _usuariosRepository;
        private readonly ILogger<LoginController> _logger;

        // Constructor con inyección de dependencias
        public LoginController(IUsuariosRepository usuariosRepository, ILogger<LoginController> logger)
        {
            _usuariosRepository = usuariosRepository;
            _logger = logger;
        }

        // Acción que devuelve la vista de login
        public IActionResult Index()
        {
            var model = new LoginViewModel
            {
                IsAuthenticated = HttpContext.Session.GetString("IsAuthenticated") == "true"
            };
            return View(model); // Retornar el modelo a la vista de login
        }

        // Acción para procesar el login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                model.ErrorMessage = "Por favor ingrese su nombre de usuario y contraseña.";
                return View(model);
            }

            // Usamos el repositorio para obtener el usuario con el nombre de usuario y la contraseña
            var usuario = _usuariosRepository.ObtenerUsuario(model.Username, model.Password);

            if (usuario != null)
            {
                // Guardar en la sesión que el usuario está autenticado
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("UserRole", usuario.Rol);

                // Redirigir según el rol del usuario
                if (usuario.Rol == "Administrador")
                {
                    return RedirectToAction("Index", "Presupuestos");
                }
                else if (usuario.Rol == "Cliente")
                {
                    return RedirectToAction("VerDetalle", "Presupuestos");
                }
            }

            // Si las credenciales no son válidas, mostrar error
            model.ErrorMessage = "Credenciales inválidas.";
            model.IsAuthenticated = false;
            return View(model);
        }

        // Acción para cerrar sesión
        public IActionResult Logout()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();

            // Redirigir a la vista de login
            return RedirectToAction("Login");
        }
    }
}
