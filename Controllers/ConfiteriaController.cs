using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineBenavides.Data;
using CineBenavides.Models;

namespace CineBenavides.Controllers
{
    public class ConfiteriaController : Controller
    {
        private readonly CineContext _context;
        private readonly IWebHostEnvironment _env;

        public ConfiteriaController(CineContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var productos = _context.confiteria.OrderBy(p => p.Nombre).ToList();
            return View(productos);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Confiteria producto, IFormFile? imagen)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            ModelState.Remove("ImagenFile");

            if (!ModelState.IsValid)
                return View(producto);

            if (imagen != null && imagen.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(carpeta);
                var nombre = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                using var stream = new FileStream(Path.Combine(carpeta, nombre), FileMode.Create);
                imagen.CopyTo(stream);
                producto.ImagenUrl = "/images/" + nombre;
            }

            _context.confiteria.Add(producto);
            _context.SaveChanges();
            TempData["Mensaje"] = $"Producto \"{producto.Nombre}\" creado correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var producto = _context.confiteria.Find(id);
            if (producto == null) return NotFound();

            return View(producto);
        }

        [HttpPost]
        public IActionResult Edit(Confiteria producto, IFormFile? imagen)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            ModelState.Remove("ImagenFile");

            if (!ModelState.IsValid)
                return View(producto);

            if (imagen != null && imagen.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(carpeta);
                var nombre = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                using var stream = new FileStream(Path.Combine(carpeta, nombre), FileMode.Create);
                imagen.CopyTo(stream);
                producto.ImagenUrl = "/images/" + nombre;
            }
            else
            {
                var original = _context.confiteria.AsNoTracking()
                    .FirstOrDefault(p => p.Id == producto.Id);
                producto.ImagenUrl = original?.ImagenUrl;
            }

            _context.confiteria.Update(producto);
            _context.SaveChanges();
            TempData["Mensaje"] = $"Producto \"{producto.Nombre}\" actualizado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var producto = _context.confiteria.Find(id);
            if (producto != null)
            {
                _context.confiteria.Remove(producto);
                _context.SaveChanges();
                TempData["Mensaje"] = $"Producto \"{producto.Nombre}\" eliminado.";
            }

            return RedirectToAction("Index");
        }
    }
}
