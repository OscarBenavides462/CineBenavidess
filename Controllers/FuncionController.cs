using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CineBenavides.Data;
using CineBenavides.Models;

namespace CineBenavides.Controllers
{
    public class FuncionController : Controller
    {
        private readonly CineContext _context;

        public FuncionController(CineContext context)
        {
            _context = context;
        }

        private void CargarSelectLists(int? peliculaId = null, int? salaId = null)
        {
            ViewBag.Peliculas = new SelectList(
                _context.peliculas.OrderBy(p => p.Titulo).ToList(), "Id", "Titulo", peliculaId);
            ViewBag.Salas = new SelectList(
                _context.salas.OrderBy(s => s.Nombre).ToList(), "Id", "Nombre", salaId);
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var funciones = _context.funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .OrderBy(f => f.FechaHora)
                .ToList();

            return View(funciones);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (!_context.peliculas.Any())
            {
                TempData["Error"] = "Primero debes registrar al menos una película.";
                return RedirectToAction("Index");
            }
            if (!_context.salas.Any())
            {
                TempData["Error"] = "Primero debes registrar al menos una sala.";
                return RedirectToAction("Index");
            }

            CargarSelectLists();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Funcion funcion)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            // Propiedades de navegación que no vienen del form
            ModelState.Remove("Pelicula");
            ModelState.Remove("Sala");
            ModelState.Remove("Reservas");

            // Validar fecha mínima (SQL Server no acepta DateTime.MinValue)
            if (funcion.FechaHora == DateTime.MinValue || funcion.FechaHora.Year < 2000)
            {
                ModelState.AddModelError("FechaHora", "Debe ingresar una fecha y hora válida.");
            }

            if (!ModelState.IsValid)
            {
                CargarSelectLists(funcion.PeliculaId, funcion.SalaId);
                return View(funcion);
            }

            _context.funciones.Add(funcion);
            _context.SaveChanges();
            TempData["Mensaje"] = "Función creada correctamente.";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var funcion = _context.funciones.Find(id);
            if (funcion == null) return NotFound();

            CargarSelectLists(funcion.PeliculaId, funcion.SalaId);
            return View(funcion);
        }

        [HttpPost]
        public IActionResult Edit(Funcion funcion)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            ModelState.Remove("Pelicula");
            ModelState.Remove("Sala");
            ModelState.Remove("Reservas");

            if (funcion.FechaHora == DateTime.MinValue || funcion.FechaHora.Year < 2000)
            {
                ModelState.AddModelError("FechaHora", "Debe ingresar una fecha y hora válida.");
            }

            if (!ModelState.IsValid)
            {
                CargarSelectLists(funcion.PeliculaId, funcion.SalaId);
                return View(funcion);
            }

            _context.funciones.Update(funcion);
            _context.SaveChanges();
            TempData["Mensaje"] = "Función actualizada correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var funcion = _context.funciones.Find(id);
            if (funcion != null)
            {
                var reservas = _context.reservas
                    .Include(r => r.Items)
                    .Where(r => r.FuncionId == id)
                    .ToList();

                foreach (var reserva in reservas)
                {
                    _context.reservaItems.RemoveRange(reserva.Items);
                    _context.reservas.Remove(reserva);
                }

                _context.funciones.Remove(funcion);
                _context.SaveChanges();
                TempData["Mensaje"] = "Función eliminada.";
            }

            return RedirectToAction("Index");
        }
    }
}
