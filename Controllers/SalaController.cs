using Microsoft.AspNetCore.Mvc;
using CineBenavides.Data;
using CineBenavides.Models;

namespace CineBenavides.Controllers
{
    public class SalaController : Controller
    {
        private readonly CineContext _context;

        public SalaController(CineContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var salas = _context.salas.ToList();
            return View(salas);
        }

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            return View();
        }

        [HttpPost]
        public IActionResult Create(Sala sala)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _context.salas.Add(sala);
                _context.SaveChanges();

                // Generar asientos automáticamente
                GenerarAsientos(sala.Id, sala.Capacidad);
                return RedirectToAction("Index");
            }

            return View(sala);
        }

        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var sala = _context.salas.Find(id);
            if (sala == null) return NotFound();

            return View(sala);
        }

        [HttpPost]
        public IActionResult Edit(Sala sala)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                _context.salas.Update(sala);
                _context.SaveChanges();

                // Eliminar asientos anteriores y regenerar con la nueva capacidad
                var asientosViejos = _context.asientos
                    .Where(a => a.SalaId == sala.Id)
                    .ToList();
                _context.asientos.RemoveRange(asientosViejos);
                _context.SaveChanges();

                GenerarAsientos(sala.Id, sala.Capacidad);
                return RedirectToAction("Index");
            }

            return View(sala);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var sala = _context.salas.Find(id);
            if (sala != null)
            {
                // Eliminar asientos primero
                var asientos = _context.asientos.Where(a => a.SalaId == id).ToList();
                _context.asientos.RemoveRange(asientos);

                _context.salas.Remove(sala);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Método reutilizable para generar asientos
        private void GenerarAsientos(int salaId, int capacidad)
        {
            int asientosPorFila = 5;
            int totalFilas = (int)Math.Ceiling((double)capacidad / asientosPorFila);
            int asientosCreados = 0;

            for (int fila = 0; fila < totalFilas; fila++)
            {
                string letraFila = ((char)('A' + fila)).ToString();

                for (int num = 1; num <= asientosPorFila; num++)
                {
                    if (asientosCreados >= capacidad) break;

                    _context.asientos.Add(new Asiento
                    {
                        SalaId = salaId,
                        Fila   = letraFila,
                        Numero = num,
                        Tipo   = "Estándar"
                    });

                    asientosCreados++;
                }
            }

            _context.SaveChanges();
        }
    }
}
