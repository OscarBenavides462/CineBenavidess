using Microsoft.AspNetCore.Mvc;
using CineBenavides.Data;
using CineBenavides.Helpers;

namespace CineBenavides.Controllers
{
    public class LoginController : Controller
    {
        private readonly CineContext _context;
        private readonly IConfiguration _config;

        public LoginController(CineContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public IActionResult Index(string correo, string clave)
        {
            // 1. Primero revisamos el superusuario del appsettings.json
            var suCorreo = _config["SuperUsuario:Correo"];
            var suClave  = _config["SuperUsuario:Clave"];
            var suNombre = _config["SuperUsuario:Nombre"];
            var suRol    = _config["SuperUsuario:Rol"];

            if (correo == suCorreo && clave == suClave)
            {
                HttpContext.Session.SetString("Usuario", suNombre!);
                HttpContext.Session.SetString("Rol", suRol!);
                HttpContext.Session.SetInt32("UsuarioId", 0); // 0 = superusuario
                return RedirectToAction("Index", "Home");
            }

            // 2. Si no es el superusuario, buscamos en la base de datos
            var hash = HashHelpers.ObtenerHash(clave);
            var usuario = _context.usuarios
                .FirstOrDefault(u => u.Correo == correo && u.Clave == hash);

            if (usuario == null)
            {
                ViewBag.Error = "Correo o clave incorrectos.";
                return View();
            }

            HttpContext.Session.SetString("Usuario", usuario.Nombre);
            HttpContext.Session.SetString("Rol", usuario.Rol);
            HttpContext.Session.SetInt32("UsuarioId", usuario.Id);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
