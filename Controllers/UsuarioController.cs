using Microsoft.AspNetCore.Mvc;
using CineBenavides.Data;
using CineBenavides.Helpers;
using CineBenavides.Models;

namespace CineBenavides.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly CineContext _context;

        public UsuarioController(CineContext context)
        {
            _context = context;
        }

        // Lista todos los usuarios
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var usuarios = _context.usuarios.ToList();
            return View(usuarios);
        }

        // GET: muestra el formulario de creación
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            return View();
        }

        // POST: guarda el nuevo usuario
        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                // Encriptamos la clave antes de guardar
                usuario.Clave = HashHelpers.ObtenerHash(usuario.Clave);
                _context.usuarios.Add(usuario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // GET: muestra el formulario de edición
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var usuario = _context.usuarios.Find(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: actualiza el usuario
        [HttpPost]
        public IActionResult Edit(Usuario usuario)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                // Encriptamos la clave al editar también
                usuario.Clave = HashHelpers.ObtenerHash(usuario.Clave);
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(usuario);
        }

        // POST: elimina el usuario
        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var usuario = _context.usuarios.Find(id);
            if (usuario != null)
            {
                _context.usuarios.Remove(usuario);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
