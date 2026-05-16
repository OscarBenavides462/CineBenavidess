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

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            // Traemos las funciones con su película y sala
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

            ViewBag.Peliculas = new SelectList(_context.peliculas.ToList(), "Id", "Titulo");
            ViewBag.Salas     = new SelectList(_context.salas.ToList(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Funcion funcion)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _context.funciones.Add(funcion);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Peliculas = new SelectList(_context.peliculas.ToList(), "Id", "Titulo");
            ViewBag.Salas     = new SelectList(_context.salas.ToList(), "Id", "Nombre");
            return View(funcion);
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var funcion = _context.funciones.Find(id);
            if (funcion == null) return NotFound();

            ViewBag.Peliculas = new SelectList(_context.peliculas.ToList(), "Id", "Titulo", funcion.PeliculaId);
            ViewBag.Salas     = new SelectList(_context.salas.ToList(), "Id", "Nombre", funcion.SalaId);
            return View(funcion);
        }

        [HttpPost]
        public IActionResult Edit(Funcion funcion)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _context.funciones.Update(funcion);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Peliculas = new SelectList(_context.peliculas.ToList(), "Id", "Titulo", funcion.PeliculaId);
            ViewBag.Salas     = new SelectList(_context.salas.ToList(), "Id", "Nombre", funcion.SalaId);
            return View(funcion);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var funcion = _context.funciones.Find(id);
            if (funcion != null)
            {
                _context.funciones.Remove(funcion);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
