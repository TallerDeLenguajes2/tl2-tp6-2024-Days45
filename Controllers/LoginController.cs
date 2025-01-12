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

        public LoginController(IUsuariosRepository usuariosRepository, ILogger<LoginController> logger)
        {
            _usuariosRepository = usuariosRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new LoginViewModel
            {
                IsAuthenticated = HttpContext.Session.GetString("IsAuthenticated") == "true"
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                model.ErrorMessage = "Por favor ingrese su nombre de usuario y contraseña.";
                return View("Index", model);
            }

            var usuario = _usuariosRepository.ObtenerUsuario(model.Username, model.Password);

            if (usuario != null)
            {
                HttpContext.Session.SetString("IsAuthenticated", "true");
                HttpContext.Session.SetString("UserRole", usuario.Rol);
                HttpContext.Session.SetString("UserName", usuario.Nombre);
                HttpContext.Session.SetInt32("UserId", usuario.IdUsuario);

                if (usuario.Rol == "Administrador")
                {
                    return RedirectToAction("Index", "Presupuestos");
                }
                else if (usuario.Rol == "Cliente")
                {
                    // Redirige al cliente a la vista de detalle de su presupuesto
                    return RedirectToAction("VerDetalle", "Presupuestos", new { id = usuario.IdUsuario });
                }
            }

            model.ErrorMessage = "Credenciales inválidas.";
            model.IsAuthenticated = false;
            return View("Index", model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
