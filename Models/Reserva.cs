using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace CineBenavides.Models
{
    public class Confiteria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0, 500000)]
        public double Precio { get; set; }

        [StringLength(250)]
        public string? Descripcion { get; set; }

        // Ruta guardada en BD: /images/crispetas.jpg
        public string? ImagenUrl { get; set; }

        // Solo para recibir el archivo subido, NO se guarda en BD
        [NotMapped]
        public IFormFile? ImagenFile { get; set; }

        [Range(0, 1000)]
        public int Stock { get; set; }
    }

    // ─────────────────────────────────────────────

    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required]
        public int FuncionId { get; set; }
        public Funcion? Funcion { get; set; }

        [Required]
        public int AsientoId { get; set; }
        public Asiento? Asiento { get; set; }

        public DateTime FechaReserva { get; set; } = DateTime.Now;

        public string Estado { get; set; } = "Pendiente";

        public double Total { get; set; }

        public List<ReservaItem> Items { get; set; } = new();
    }

    // ─────────────────────────────────────────────

    public class ReservaItem
    {
        public int Id { get; set; }

        [Required]
        public int ReservaId { get; set; }
        public Reserva? Reserva { get; set; }

        [Required]
        public int ConfiteriaId { get; set; }
        public Confiteria? Confiteria { get; set; }

        [Range(1, 20)]
        public int Cantidad { get; set; }

        public double Subtotal { get; set; }
    }
}