using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var salas = _context.salas.Include(s => s.Asientos).OrderBy(s => s.Nombre).ToList();
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

            ModelState.Remove("Asientos");
            ModelState.Remove("Funciones");

            if (!ModelState.IsValid)
                return View(sala);

            _context.salas.Add(sala);
            _context.SaveChanges();
            GenerarAsientos(sala.Id, sala.Capacidad);
            TempData["Mensaje"] = $"Sala \"{sala.Nombre}\" creada con {sala.Capacidad} asientos.";
            return RedirectToAction("Index");
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

            ModelState.Remove("Asientos");
            ModelState.Remove("Funciones");

            if (!ModelState.IsValid)
                return View(sala);

            _context.salas.Update(sala);
            _context.SaveChanges();

            var asientosActuales = _context.asientos.Where(a => a.SalaId == sala.Id).ToList();
            if (asientosActuales.Count != sala.Capacidad)
            {
                _context.asientos.RemoveRange(asientosActuales);
                _context.SaveChanges();
                GenerarAsientos(sala.Id, sala.Capacidad);
            }

            TempData["Mensaje"] = $"Sala \"{sala.Nombre}\" actualizada.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var sala = _context.salas.Find(id);
            if (sala != null)
            {
                var funciones = _context.funciones.Where(f => f.SalaId == id).ToList();
                foreach (var funcion in funciones)
                {
                    var reservas = _context.reservas.Include(r => r.Items).Where(r => r.FuncionId == funcion.Id).ToList();
                    foreach (var reserva in reservas)
                    {
                        _context.reservaItems.RemoveRange(reserva.Items);
                        _context.reservas.Remove(reserva);
                    }
                }
                _context.funciones.RemoveRange(funciones);
                var asientos = _context.asientos.Where(a => a.SalaId == id).ToList();
                _context.asientos.RemoveRange(asientos);
                _context.salas.Remove(sala);
                _context.SaveChanges();
                TempData["Mensaje"] = $"Sala \"{sala.Nombre}\" eliminada.";
            }

            return RedirectToAction("Index");
        }

        private void GenerarAsientos(int salaId, int capacidad)
        {
            int porFila = 10;
            int totalFilas = (int)Math.Ceiling((double)capacidad / porFila);
            int creados = 0;

            for (int f = 0; f < totalFilas && creados < capacidad; f++)
            {
                string fila = ((char)('A' + f)).ToString();
                for (int n = 1; n <= porFila && creados < capacidad; n++)
                {
                    string tipo = f < 2 ? "Preferencial" : (f == totalFilas - 1 ? "VIP" : "Estándar");
                    _context.asientos.Add(new Asiento { SalaId = salaId, Fila = fila, Numero = n, Tipo = tipo });
                    creados++;
                }
            }
            _context.SaveChanges();
        }
    }
}
