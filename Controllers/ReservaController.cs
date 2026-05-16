using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CineBenavides.Data;
using CineBenavides.Models;

namespace CineBenavides.Controllers
{
    public class ReservaController : Controller
    {
        private readonly CineContext _context;

        public ReservaController(CineContext context)
        {
            _context = context;
        }

        // Lista de todas las reservas (Soporte y Admin)
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Soporte" && rol != "Administrador")
                return RedirectToAction("Index", "Home");

            var reservas = _context.reservas
                .Include(r => r.Usuario)
                .Include(r => r.Funcion).ThenInclude(f => f.Pelicula)
                .Include(r => r.Funcion).ThenInclude(f => f.Sala)
                .Include(r => r.Asiento)
                .OrderByDescending(r => r.FechaReserva)
                .ToList();

            return View(reservas);
        }

        // Mis reservas (cada usuario ve las suyas)
        public IActionResult MisReservas()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            // El superusuario (Id=0) ve todas las reservas
            var reservas = usuarioId == 0
                ? _context.reservas
                    .Include(r => r.Funcion).ThenInclude(f => f.Pelicula)
                    .Include(r => r.Funcion).ThenInclude(f => f.Sala)
                    .Include(r => r.Asiento)
                    .Include(r => r.Items).ThenInclude(i => i.Confiteria)
                    .OrderByDescending(r => r.FechaReserva)
                    .ToList()
                : _context.reservas
                    .Include(r => r.Funcion).ThenInclude(f => f.Pelicula)
                    .Include(r => r.Funcion).ThenInclude(f => f.Sala)
                    .Include(r => r.Asiento)
                    .Include(r => r.Items).ThenInclude(i => i.Confiteria)
                    .Where(r => r.UsuarioId == usuarioId)
                    .OrderByDescending(r => r.FechaReserva)
                    .ToList();

            return View(reservas);
        }

        // GET: Paso 1 — elegir función
        public IActionResult Crear()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var funciones = _context.funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .Where(f => f.FechaHora >= DateTime.Today)
                .OrderBy(f => f.FechaHora)
                .ToList();

            return View(funciones);
        }

        // GET: Paso 2 — elegir asiento y confitería
        public IActionResult Confirmar(int funcionId)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var funcion = _context.funciones
                .Include(f => f.Pelicula)
                .Include(f => f.Sala)
                .FirstOrDefault(f => f.Id == funcionId);

            if (funcion == null) return NotFound();

            // Asientos ocupados en esta función
            var asientosOcupados = _context.reservas
                .Where(r => r.FuncionId == funcionId)
                .Select(r => r.AsientoId)
                .ToList();

            // Asientos disponibles de la sala
            var asientosDisponibles = _context.asientos
                .Where(a => a.SalaId == funcion.SalaId && !asientosOcupados.Contains(a.Id))
                .OrderBy(a => a.Fila).ThenBy(a => a.Numero)
                .ToList();

            var confiteria = _context.confiteria
                .Where(c => c.Stock > 0)
                .ToList();

            ViewBag.Funcion = funcion;
            ViewBag.AsientosDisponibles = new SelectList(
                asientosDisponibles.Select(a => new {
                    a.Id,
                    Descripcion = $"Fila {a.Fila} - Asiento {a.Numero} ({a.Tipo})"
                }),
                "Id", "Descripcion"
            );
            ViewBag.Confiteria = confiteria;

            return View();
        }

        // POST: Guardar reserva completa
        [HttpPost]
        public IActionResult Confirmar(int funcionId, int asientoId,
                                       List<int> confiteriaIds, List<int> cantidades)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var funcion = _context.funciones.Find(funcionId);
            if (funcion == null) return NotFound();

            // Verificar que el asiento siga disponible
            var yaReservado = _context.reservas
                .Any(r => r.FuncionId == funcionId && r.AsientoId == asientoId);

            if (yaReservado)
            {
                TempData["Error"] = "Ese asiento ya fue reservado. Por favor elige otro.";
                return RedirectToAction("Confirmar", new { funcionId });
            }

            // Calcular total
            double total = funcion.Precio;
            var items = new List<ReservaItem>();

            if (confiteriaIds != null)
            {
                for (int i = 0; i < confiteriaIds.Count; i++)
                {
                    if (cantidades[i] > 0)
                    {
                        var producto = _context.confiteria.Find(confiteriaIds[i]);
                        if (producto != null && producto.Stock >= cantidades[i])
                        {
                            var subtotal = producto.Precio * cantidades[i];
                            total += subtotal;

                            items.Add(new ReservaItem
                            {
                                ConfiteriaId = confiteriaIds[i],
                                Cantidad     = cantidades[i],
                                Subtotal     = subtotal
                            });

                            producto.Stock -= cantidades[i];
                            _context.confiteria.Update(producto);
                        }
                    }
                }
            }

            // Obtener UsuarioId — si es superusuario (0) guardamos null-safe
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;

            // Si es superusuario no tiene FK en usuarios, usamos el primer usuario real
            // o simplemente guardamos 0 y manejamos en la vista
            if (usuarioId == 0)
            {
                var primerUsuario = _context.usuarios.FirstOrDefault();
                usuarioId = primerUsuario?.Id ?? 0;
            }

            var reserva = new Reserva
            {
                UsuarioId    = usuarioId,
                FuncionId    = funcionId,
                AsientoId    = asientoId,
                FechaReserva = DateTime.Now,
                Estado       = "Confirmada",
                Total        = total,
                Items        = items
            };

            _context.reservas.Add(reserva);
            _context.SaveChanges();

            TempData["Exito"] = "¡Reserva realizada con éxito!";
            return RedirectToAction("MisReservas");
        }

        // POST: Cancelar reserva
        [HttpPost]
        public IActionResult Cancelar(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            var reserva = _context.reservas
                .Include(r => r.Items)
                .FirstOrDefault(r => r.Id == id);

            if (reserva != null)
            {
                // Devolver stock de confitería
                foreach (var item in reserva.Items)
                {
                    var producto = _context.confiteria.Find(item.ConfiteriaId);
                    if (producto != null)
                    {
                        producto.Stock += item.Cantidad;
                        _context.confiteria.Update(producto);
                    }
                }

                reserva.Estado = "Cancelada";
                _context.reservas.Update(reserva);
                _context.SaveChanges();
            }

            var rol = HttpContext.Session.GetString("Rol");
            return rol == "Administrador" || rol == "Soporte"
                ? RedirectToAction("Index")
                : RedirectToAction("MisReservas");
        }
    }
}
