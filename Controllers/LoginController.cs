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
        private readonly IClienteRepository _repositorioCliente;
        private readonly ILogger<LoginController> _logger;


        public LoginController(IUsuariosRepository usuariosRepository, ILogger<LoginController> logger, IClienteRepository repositorioCliente)
        {
            _usuariosRepository = usuariosRepository;
            _logger = logger;
            _repositorioCliente = repositorioCliente;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var isAuthenticated = HttpContext.Session.GetString("IsAuthenticated");
            var rol = HttpContext.Session.GetString("UserRole");

            if (isAuthenticated == "true" && !string.IsNullOrEmpty(rol))
            {
                return RedirectToAction("Index", "Presupuestos");
            }

            return View(new LoginViewModel { IsAuthenticated = false });
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            _logger.LogInformation($"Intento de login con usuario: {model.Username}");

            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewData["Error"] = "Debe ingresar un usuario y una contraseña.";
                return View("Index", new LoginViewModel { IsAuthenticated = false });
            }

            var user = _usuariosRepository.ObtenerUsuario(model.Username, model.Password);

            if (user != null)
            {
                _logger.LogInformation($"Usuario autenticado: {user.Usuario} con rol {user.Rol}");

                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("UserRole", user.Rol);
                HttpContext.Session.SetInt32("UserId", user.IdUsuario);

                return RedirectToAction("Index", "Presupuestos");
            }

            _logger.LogWarning("Intento de login fallido: Usuario o contraseña incorrectos.");
            ViewData["Error"] = "Usuario o contraseña incorrectos";
            return View("Index", new LoginViewModel { IsAuthenticated = false });
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
