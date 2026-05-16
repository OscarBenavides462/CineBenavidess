using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineBenavides.Data;

namespace CineBenavides.Controllers
{
    public class HomeController : Controller
    {
        private readonly CineContext _context;
        public HomeController(CineContext context) { _context = context; }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            // Funciones de hoy en adelante para mostrar en el home
            var funciones = _context.funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .Where(f => f.FechaHora >= DateTime.Today)
                .OrderBy(f => f.FechaHora)
                .Take(6)
                .ToList();

            return View(funciones);
        }
    }
}
