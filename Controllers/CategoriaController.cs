using Microsoft.AspNetCore.Mvc;
using CineBenavides.Data;
using CineBenavides.Models;

namespace CineBenavides.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly CineContext _context;

        public CategoriaController(CineContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var categorias = _context.categorias.OrderBy(c => c.Nombre).ToList();
            return View(categorias);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Categoria categoria)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            // Lista de navegación no viene del form
            ModelState.Remove("Peliculas");

            if (!ModelState.IsValid)
                return View(categoria);

            _context.categorias.Add(categoria);
            _context.SaveChanges();
            TempData["Mensaje"] = $"Categoría \"{categoria.Nombre}\" creada correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var categoria = _context.categorias.Find(id);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        [HttpPost]
        public IActionResult Edit(Categoria categoria)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            ModelState.Remove("Peliculas");

            if (!ModelState.IsValid)
                return View(categoria);

            _context.categorias.Update(categoria);
            _context.SaveChanges();
            TempData["Mensaje"] = $"Categoría \"{categoria.Nombre}\" actualizada.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var categoria = _context.categorias.Find(id);
            if (categoria != null)
            {
                // Verificar si tiene películas asociadas
                var tienePeliculas = _context.peliculas.Any(p => p.CategoriaId == id);
                if (tienePeliculas)
                {
                    TempData["Error"] = "No se puede eliminar la categoría porque tiene películas asociadas.";
                    return RedirectToAction("Index");
                }

                _context.categorias.Remove(categoria);
                _context.SaveChanges();
                TempData["Mensaje"] = $"Categoría \"{categoria.Nombre}\" eliminada.";
            }

            return RedirectToAction("Index");
        }
    }
}
