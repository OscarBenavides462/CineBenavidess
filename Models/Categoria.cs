using System.ComponentModel.DataAnnotations;

namespace CineBenavides.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(250)]
        public string Descripcion { get; set; }

        // Navegación
        public List<Pelicula> Peliculas { get; set; } = new();
    }
}
