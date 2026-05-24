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
        private readonly IWebHostEnvironment _env;

        public PeliculaController(CineContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private void CargarCategorias(int? seleccionada = null)
        {
            ViewBag.Categorias = new SelectList(
                _context.categorias.OrderBy(c => c.Nombre).ToList(),
                "Id", "Nombre", seleccionada);
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var peliculas = _context.peliculas
                .Include(p => p.Categoria)
                .OrderBy(p => p.Titulo)
                .ToList();

            return View(peliculas);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            CargarCategorias();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Pelicula pelicula, IFormFile? imagen)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            // Quitar propiedades de navegación que el form no envía
            ModelState.Remove("Categoria");
            ModelState.Remove("Funciones");
            ModelState.Remove("PosterFile");

            if (!ModelState.IsValid)
            {
                CargarCategorias(pelicula.CategoriaId);
                return View(pelicula);
            }

            if (imagen != null && imagen.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(carpeta); // crea la carpeta si no existe
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);
                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                imagen.CopyTo(stream);
                pelicula.PosterUrl = "/images/" + nombreArchivo;
            }

            _context.peliculas.Add(pelicula);
            _context.SaveChanges();
            TempData["Mensaje"] = $"Película \"{pelicula.Titulo}\" creada correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var pelicula = _context.peliculas.Find(id);
            if (pelicula == null) return NotFound();

            CargarCategorias(pelicula.CategoriaId);
            return View(pelicula);
        }

        [HttpPost]
        public IActionResult Edit(Pelicula pelicula, IFormFile? imagen)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            ModelState.Remove("Categoria");
            ModelState.Remove("Funciones");
            ModelState.Remove("PosterFile");

            if (!ModelState.IsValid)
            {
                CargarCategorias(pelicula.CategoriaId);
                return View(pelicula);
            }

            if (imagen != null && imagen.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "images");
                Directory.CreateDirectory(carpeta);
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);
                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                imagen.CopyTo(stream);
                pelicula.PosterUrl = "/images/" + nombreArchivo;
            }
            else
            {
                // Mantener el poster anterior si no se sube uno nuevo
                var original = _context.peliculas.AsNoTracking()
                    .FirstOrDefault(p => p.Id == pelicula.Id);
                pelicula.PosterUrl = original?.PosterUrl;
            }

            _context.peliculas.Update(pelicula);
            _context.SaveChanges();
            TempData["Mensaje"] = $"Película \"{pelicula.Titulo}\" actualizada correctamente.";
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
                var funciones = _context.funciones.Where(f => f.PeliculaId == id).ToList();
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
                _context.funciones.RemoveRange(funciones);
                _context.peliculas.Remove(pelicula);
                _context.SaveChanges();
                TempData["Mensaje"] = $"Película \"{pelicula.Titulo}\" eliminada.";
            }

            return RedirectToAction("Index");
        }
    }
}
