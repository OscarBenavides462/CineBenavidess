using System.ComponentModel.DataAnnotations;

namespace CineBenavides.Models
{
    public class Sala
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la sala es obligatorio.")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La capacidad es obligatoria.")]
        [Range(1, 500, ErrorMessage = "La capacidad debe estar entre 1 y 500.")]
        public int Capacidad { get; set; }

        // Navegación
        public List<Asiento> Asientos { get; set; } = new();
        public List<Funcion> Funciones { get; set; } = new();
    }
}
