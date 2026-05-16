using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace CineBenavides.Models
{
    public class Pelicula
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150)]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El director es obligatorio.")]
        [StringLength(100)]
        public string Director { get; set; }

        [Required(ErrorMessage = "La duración es obligatoria.")]
        [Range(1, 300, ErrorMessage = "La duración debe estar entre 1 y 300 minutos.")]
        public int DuracionMinutos { get; set; }

        [Required(ErrorMessage = "La clasificación es obligatoria.")]
        public string Clasificacion { get; set; }

        // Ruta guardada en BD: /images/batman.jpg
        public string? PosterUrl { get; set; }

        // Solo para recibir el archivo subido, NO se guarda en BD
        [NotMapped]
        public IFormFile? PosterFile { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        public List<Funcion> Funciones { get; set; } = new();
    }
}