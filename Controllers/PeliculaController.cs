using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CineBenavides.Data;
using CineBenavides.Models;

namespace CineBenavides.Controllers
{
    public class PeliculaController : Controller
    {
        private readonly CineContext _context;

        public PeliculaController(CineContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var peliculas = _context.peliculas
                .Include(p => p.Categoria)
                .ToList();

            return View(peliculas);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            ViewBag.Categorias = new SelectList(_context.categorias.ToList(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Pelicula pelicula, IFormFile imagen)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (imagen != null)
            {
                var ruta = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/images", imagen.FileName);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    imagen.CopyTo(stream);
                }

                pelicula.PosterUrl = "/images/" + imagen.FileName;
            }

            _context.peliculas.Add(pelicula);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var pelicula = _context.peliculas.Find(id);
            if (pelicula == null) return NotFound();

            ViewBag.Categorias = new SelectList(_context.categorias.ToList(), "Id", "Nombre", pelicula.CategoriaId);
            return View(pelicula);
        }

        [HttpPost]
        public IActionResult Edit(Pelicula pelicula, IFormFile imagen)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (imagen != null)
            {
                var ruta = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/images", imagen.FileName);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    imagen.CopyTo(stream);
                }

                pelicula.PosterUrl = "/images/" + imagen.FileName;
            }
            else
            {
                var original = _context.peliculas.AsNoTracking()
                    .FirstOrDefault(p => p.Id == pelicula.Id);
                pelicula.PosterUrl = original?.PosterUrl;
            }

            _context.peliculas.Update(pelicula);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var pelicula = _context.peliculas.Find(id);
            if (pelicula != null)
            {
                // Primero eliminamos las reservas de cada función
                var funciones = _context.funciones
                    .Where(f => f.PeliculaId == id)
                    .ToList();

                foreach (var funcion in funciones)
                {
                    var reservas = _context.reservas
                        .Include(r => r.Items)
                        .Where(r => r.FuncionId == funcion.Id)
                        .ToList();

                    foreach (var reserva in reservas)
                    {
                        _context.reservaItems.RemoveRange(reserva.Items);
                        _context.reservas.Remove(reserva);
                    }
                }

                // Luego eliminamos las funciones
                _context.funciones.RemoveRange(funciones);

                // Finalmente eliminamos la película
                _context.peliculas.Remove(pelicula);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
