using System.ComponentModel.DataAnnotations;

namespace CineBenavides.Models
{
    public class Funcion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una película.")]
        public int PeliculaId { get; set; }
        public Pelicula? Pelicula { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una sala.")]
        public int SalaId { get; set; }
        public Sala? Sala { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime FechaHora { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0, 1000000, ErrorMessage = "El precio debe ser mayor a 0.")]
        public double Precio { get; set; }

        // Navegación
        public List<Reserva> Reservas { get; set; } = new();
    }
}
