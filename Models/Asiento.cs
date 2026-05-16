using System.ComponentModel.DataAnnotations;

namespace CineBenavides.Models
{
    public class Asiento
    {
        public int Id { get; set; }

        [Required]
        public int SalaId { get; set; }
        public Sala? Sala { get; set; }

        [Required(ErrorMessage = "La fila es obligatoria.")]
        [StringLength(5)]
        public string Fila { get; set; } // A, B, C...

        [Required(ErrorMessage = "El número es obligatorio.")]
        [Range(1, 50)]
        public int Numero { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio.")]
        public string Tipo { get; set; } // Estándar, VIP, Preferencial
    }
}
