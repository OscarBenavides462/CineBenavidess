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

            var categorias = _context.categorias.ToList();
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

            if (ModelState.IsValid)
            {
                _context.categorias.Add(categoria);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoria);
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

            if (ModelState.IsValid)
            {
                _context.categorias.Update(categoria);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(categoria);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var categoria = _context.categorias.Find(id);
            if (categoria != null)
            {
                _context.categorias.Remove(categoria);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
